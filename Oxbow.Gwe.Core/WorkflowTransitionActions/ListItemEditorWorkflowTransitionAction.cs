using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    public class ListItemEditorWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string TargetFieldExpression { get; set; }

        [DataMember]
        public string NewValueExpression { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<ListItemEditorWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/ListItemEditorWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<ListItemEditorWorkflowTransitionAction>)userControl).UpdateDataModel(this); }
        public override string GetTypeName() { return "List Editor"; }

        public override void Execute(SPListItem spListItem)
        {
            var container = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration);
            try
            {
                var value = ResolveType.Instance.Of<ITemplateEngine>().Render(NewValueExpression, container);
                var fieldName = ResolveType.Instance.Of<ITemplateEngine>().Render(TargetFieldExpression, container);
                container.SpListItem.Web.AllowUnsafeUpdates = true;
                container.SpListItem[fieldName] = value;
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Error with the list item modifier 'splistitem[{0}] = \"{2}\"': {1}", TargetFieldExpression, exp, NewValueExpression));
            }
            try
            {
                Thread.Sleep(2000);
                container.PerformSystemSave();
                Thread.Sleep(2000);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Error when saving the list item [{0}]: {1}", spListItem.Title, exp));
            }
        }
    }
}