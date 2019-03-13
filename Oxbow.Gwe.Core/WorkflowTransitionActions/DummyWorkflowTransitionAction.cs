using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    [DataContract]
    public class DummyWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string DummyProperty { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<DummyWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/DummyWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<DummyWorkflowTransitionAction>)userControl).UpdateDataModel(this); }
        public override string GetTypeName() { return "DummyAction"; }
        public override void Execute(SPListItem spListItem) { throw new System.NotImplementedException(); }
    }
}