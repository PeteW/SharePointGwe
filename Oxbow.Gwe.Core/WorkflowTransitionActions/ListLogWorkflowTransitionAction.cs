using System;
using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    [DataContract]
    public class ListLogWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string ListName { get; set; }
        [DataMember]
        public string LogMessage { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<ListLogWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/ListLogWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<ListLogWorkflowTransitionAction>)userControl).UpdateDataModel(this); }
        public override string GetTypeName() { return "Log to list"; }
        public override void Execute(SPListItem spListItem)
        {
            if (string.IsNullOrEmpty(LogMessage) || string.IsNullOrEmpty(ListName))
                return;
            try
            {
                SPList spList;
                try
                {
                    spList = spListItem.Web.Lists[ListName];
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Unable to find the logging list named: {0}: {1}", ListName, exp));
                }
                var container = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration);
                var logMessage = ResolveType.Instance.Of<ITemplateEngine>().Render(LogMessage, container);
                var listItem = spList.Items.Add();
                listItem["Title"] = spListItem.Title;
                listItem["Description"] = logMessage;
                listItem.Web.AllowUnsafeUpdates = true;
                listItem.Update();
                listItem.Web.AllowUnsafeUpdates = false;
            }
            catch (Exception exp)
            {
                throw new Exception("The LoggingWorkflowTransitionHandler ran into the following issue: " + exp);
            }
    
        }
    }
}