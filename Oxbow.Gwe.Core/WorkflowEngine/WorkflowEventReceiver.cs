using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.TemplateEngine;
using Oxbow.Gwe.Core.Utils;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.Core.WorkflowEngine
{
    public class WorkflowEventReceiver: SPItemEventReceiver
    {
        private static string lastFormName = string.Empty;
        private static DateTime lastFormTime = DateTime.MinValue;

        private static bool IsDuplicateItemUpdate(SPItemEventProperties properties)
        {
            //if we visited the same form within 5 seconds return true
            if (lastFormName == properties.AfterUrl && (DateTime.Now - lastFormTime).TotalSeconds < 5)
                return true;
            lastFormName = properties.AfterUrl;
            lastFormTime = DateTime.Now;
            return false;
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            ResolveType.Instance.Of<ILogger>().Debug("ItemAdded called.");
            RunWorkflow(properties);
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            ResolveType.Instance.Of<ILogger>().Debug("ItemUpdated called.");
            RunWorkflow(properties);
        }

        private bool IsFormEmpty(SPListItem spListItem)
        {
            var formFileBytes = spListItem.File.OpenBinary();
            var xmlString = new UTF8Encoding().GetString(formFileBytes).Trim();
            return string.IsNullOrEmpty(xmlString);
        }
        
        private void RunWorkflow(SPItemEventProperties properties)
        {
            if (IsFormEmpty(properties.ListItem))
            {
                ResolveType.Instance.Of<ILogger>().Info("Form was empty. Ignoring the event.");
                return;
            }
            if (IsDuplicateItemUpdate(properties))
            {
                ResolveType.Instance.Of<ILogger>().Debug("Duplicate item event detected. Ignoring the event.");
                return;
            }
            DisableEventFiring();
            ResolveType.Instance.Of<ILogger>().Debug(string.Format("Running workflow for ListItem #{0} named [{1}] of list [{2}] in site [{3}]", properties.ListItem.ID, properties.ListItem.Name, properties.List.Title, properties.WebUrl));
            using (SPWeb webContext = properties.OpenWeb())
            {
                try
                {
                    var workflowConfiguration = new WorkflowConfigurationRepository(webContext).GetWorkflowConfigurationByListName(properties.ListTitle);
                    var container = ResolveType.Instance.OfSpListItemContainer(properties.ListItem, workflowConfiguration);
                    var selectedAction = ResolveType.Instance.Of<ITemplateEngine>().Render(workflowConfiguration.SelectedActionExpression, container);
//                    var selectedAction = formContainer.GetValueByXpath(workflow.SelectedActionXPath, string.Empty);
                    if(string.IsNullOrEmpty(selectedAction))
                    {
                        ResolveType.Instance.Of<ILogger>().Debug(string.Format("No value found within the selected action [{0}]. No action will be taken.", workflowConfiguration.SelectedActionExpression));
                        //log the update event here
                    }
                    else
                    {
                        try
                        {
                            var transition = workflowConfiguration.WorkflowTransitions.Where(x => x.Name == selectedAction).FirstOrDefault();
                            if (transition == null)
                            {
                                ResolveType.Instance.Of<ILogger>().Error(string.Format("No transition found for the selected action [{0}]. No action will be taken.", selectedAction));
                                //issue an error here?
                            }
                            else
                            {
                                //we need this thread sleep statement to ensure there are no locks on the existing file before we perform changes
                                Thread.Sleep(8000);
                                var workflowTransitionActionElements = transition.WorkflowTransitionActionElements.OrderBy(x => x.OrderId);
                                ResolveType.Instance.Of<ILogger>().Debug("Beginning action chain");
                                foreach (var workflowTransitionActionElement in workflowTransitionActionElements)
                                {
                                    var workflowTransitionAction = WorkflowTransitionActionFactory.GetWorkflowTransitionAction(workflowTransitionActionElement, workflowConfiguration);
                                    ResolveType.Instance.Of<ILogger>().Debug(string.Format("Running action [{0}] of type [{1}]", workflowTransitionActionElement.Name, workflowTransitionAction.GetType()));
                                    try
                                    {
                                        if(!string.IsNullOrEmpty(workflowTransitionActionElement.ExecuteConditionExpression))
                                        {
                                            var conditionVal = ResolveType.Instance.Of<ITemplateEngine>().Render(workflowTransitionActionElement.ExecuteConditionExpression, container)??"";
                                            if(!conditionVal.IsGweTrue())
                                            {
                                                ResolveType.Instance.Of<ILogger>().Debug(string.Format("[{0}] evaluated to [{1}] which was not true nor 1 so this action will be skipped.", workflowTransitionActionElement.ExecuteConditionExpression, conditionVal));
                                                continue;
                                            }
                                        }
                                        workflowTransitionAction.Execute(properties.ListItem.GetLatestCopy());
                                    }
                                    catch(Exception exp)
                                    {
                                        ResolveType.Instance.Of<ILogger>().Error("Workflow action failed: "+ exp);
                                        if (workflowTransitionActionElement.HaltOnFailure)
                                            throw exp;
                                        continue;
                                    }
                                }
                            }
                        }
                        catch(Exception exp)
                        {
                            MailWorkflowTransitionHandler.SendWorkflowErrorMessage(properties.ListItem, workflowConfiguration, exp);
                            throw exp;
                        }
                    }
                }
                catch(Exception exp)
                {
                    ResolveType.Instance.Of<ILogger>().Error(string.Format("Error occurred in the midst of the workflow: {0}", exp));
                }
            }
            EnableEventFiring();
        }
    }
}