using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.SharePoint.CONTROLTEMPLATES.Oxbow.Gwe
{
    public partial class ListItemEditorWorkflowTransitionActionConfig : UserControl, IWorkflowTransitionActionConfigControl<ListItemEditorWorkflowTransitionAction>
    {
        public void UpdateUi(ListItemEditorWorkflowTransitionAction workflowTransitionAction)
        {
            txtTargetFieldExpression.Text = workflowTransitionAction.TargetFieldExpression;
            txtNewValueExpression.Text = workflowTransitionAction.NewValueExpression;
        }

        public void UpdateDataModel(ListItemEditorWorkflowTransitionAction workflowTransitionAction)
        {
            workflowTransitionAction.NewValueExpression = txtNewValueExpression.Text;
            workflowTransitionAction.TargetFieldExpression = txtTargetFieldExpression.Text;
        }
    }
}
