using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.SharePoint.Controls
{
    public partial class PermissionSetWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<PermissionSetWorkflowTransitionAction>
    {
        public void UpdateUi(PermissionSetWorkflowTransitionAction workflowTransitionAction) { txtPermissionSet.Text = workflowTransitionAction.PermissionSet; }
        public void UpdateDataModel(PermissionSetWorkflowTransitionAction workflowTransitionAction) { workflowTransitionAction.PermissionSet = txtPermissionSet.Text; }
    }
}