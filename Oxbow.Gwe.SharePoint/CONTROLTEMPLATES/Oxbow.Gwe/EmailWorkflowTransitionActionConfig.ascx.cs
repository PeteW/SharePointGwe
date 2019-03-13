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
    public partial class EmailWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<EmailWorkflowTransitionAction>
    {
        public void UpdateUi(EmailWorkflowTransitionAction workflowTransitionAction) { 
            txtBody.Text = workflowTransitionAction.Body;
            txtFrom.Text = workflowTransitionAction.From;
            txtSubject.Text = workflowTransitionAction.Subject;
            txtTargetCc.Text = workflowTransitionAction.TargetCc;
            txtTargetRecipients.Text = workflowTransitionAction.TargetRecipients;
        }
        public void UpdateDataModel(EmailWorkflowTransitionAction workflowTransitionAction)
        {
            workflowTransitionAction.Body = txtBody.Text;
            workflowTransitionAction.From = txtFrom.Text;
            workflowTransitionAction.Subject = txtSubject.Text;
            workflowTransitionAction.TargetCc = txtTargetCc.Text;
            workflowTransitionAction.TargetRecipients = txtTargetRecipients.Text;
        }
    }
}