using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.TemplateEngine;
using Oxbow.Gwe.Core.Utils;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Oxbow.Gwe.Core.WorkflowEngine
{
    public class GweTimerJobDefinition : SPJobDefinition
    {
        public GweTimerJobDefinition(string name, SPWebApplication webApplication)
            : base(name, webApplication, null, SPJobLockType.ContentDatabase)
        {
            Title = SettingsManager.ApplicationName;
        }

        //you need to have a default constructor so it can be serialized
        public GweTimerJobDefinition()
        {
        }

        public override void Execute(Guid targetInstanceId)
        {
            var webApp = Parent as SPWebApplication;
            var logs = GweAgentRunner.RunJob(webApp, TimeTriggerJobRunner.RunTimerJob);
        }
    }

    public class TimeTriggerJobRunner
    {
        public static IList<AgentLogItem> RunTimerJob(WorkflowConfiguration workflowConfiguration, SPList list)
        {
            var result = new List<AgentLogItem>();
            foreach (var workflowTimeTrigger in workflowConfiguration.WorkflowTimeTriggers.OrderBy(x => x.OrderId))
            {
                var viewName = workflowTimeTrigger.ViewName;
                var view = list.GetSpView(viewName);
                var items = list.GetItems(view);
                ResolveType.Instance.Of<ILogger>().Debug("Running timer on list [" + list.Title + "] view [" + viewName + "]");
                foreach (SPListItem spListItem in items)
                {
                    SPListItem latestVersion = spListItem.GetLatestCopy();
                    var container = ResolveType.Instance.OfSpListItemContainer(latestVersion, workflowConfiguration);
                    //set the transition to execute as infopath
                    if (workflowConfiguration.SelectedActionExpression.ToLower().Contains("!xpath"))
                    {
                        var formContainer = container as IFormContainer;
                        var transition = ResolveType.Instance.Of<ITemplateEngine>().Render(workflowTimeTrigger.TransitionToExecute, container);
                        var xpath = ResolveType.Instance.Of<ITemplateEngine>().GetOperandsFromFunctionCall(workflowConfiguration.SelectedActionExpression, formContainer).FirstOrDefault();
                        var info = string.Format("Updating the listitem [{0}] setting the field [{1}] to transition [{2}]", spListItem.Url, xpath, workflowTimeTrigger.TransitionToExecute);
                        result.Add(AgentLogItem.Info(info));
                        formContainer.AssertNotNull(string.Format("The workflowTimeTrigger.SelectedActionExpression [{0}] does not inherit from Form", workflowTimeTrigger.Name));
                        formContainer.SetNodeByXpath(xpath, transition);
                        formContainer.PerformSystemSave();
                    }
                    //set the transition to execute as a list item
                    else if (workflowConfiguration.SelectedActionExpression.ToLower().Contains("!listitemfield"))
                    {
                        var transition = ResolveType.Instance.Of<ITemplateEngine>().Render(workflowTimeTrigger.TransitionToExecute, container);
                        var listItemFieldName = ResolveType.Instance.Of<ITemplateEngine>().GetOperandsFromFunctionCall(workflowConfiguration.SelectedActionExpression, container).FirstOrDefault();
                        try
                        {
                            var info = string.Format("Updating the listitem [{0}] setting the field [{1}] to transition [{2}]", spListItem.Url, listItemFieldName, workflowTimeTrigger.TransitionToExecute);
                            result.Add(AgentLogItem.Info(info));
                            container.SpListItem[listItemFieldName] = transition;
                            container.PerformSystemSave();
                        }
                        catch (Exception exp)
                        {
                            var err = string.Format("The workflowConfiguration.SelectedActionExpression [{1}] contains a '!listitemfield' function: [{0}] however an exception happens when we attempt to set listitem[{2}] to the new transition: {3}", workflowConfiguration.SelectedActionExpression, workflowTimeTrigger.Name, listItemFieldName, exp);
                            result.Add(AgentLogItem.Error(err));
                        }
                    }
                    else
                    {
                        throw new Exception("The workflorConfiguration.SelectedActionExpression must start with !xpath or !listitemfield, otherwise we wont be able to write back to the value.");
                    }
                    result.Add(AgentLogItem.Info(string.Format("The time trigger [{0}] was activated. Running transition [{1}] on list item [{2}]", workflowTimeTrigger.Name, workflowTimeTrigger.TransitionToExecute, spListItem.Url)));
                }
            }
            return result;
        }
    }
}