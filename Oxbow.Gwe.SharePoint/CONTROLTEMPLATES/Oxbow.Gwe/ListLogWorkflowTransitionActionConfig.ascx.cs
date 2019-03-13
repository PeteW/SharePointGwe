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
    public partial class ListLogWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<ListLogWorkflowTransitionAction>
    {
        public void UpdateUi(ListLogWorkflowTransitionAction workflowTransitionAction)
        {
            txtListName.Text = workflowTransitionAction.ListName;
            txtLogMessage.Text = workflowTransitionAction.LogMessage;
        }
        public void UpdateDataModel(ListLogWorkflowTransitionAction workflowTransitionAction)
        {
            workflowTransitionAction.ListName = txtListName.Text;
            workflowTransitionAction.LogMessage = txtLogMessage.Text;
        }
    }
}