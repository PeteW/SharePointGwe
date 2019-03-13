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
    public partial class InjectXmlWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<InjectXmlWorkflowTransitionAction>
    {
        public void UpdateUi(InjectXmlWorkflowTransitionAction workflowTransitionAction)
        {
            txtTargetXPath.Text = workflowTransitionAction.TargetFieldXpath;
            txtXml.Text = workflowTransitionAction.Xml;
            rbPrependXmlNo.Checked = !workflowTransitionAction.PrependXml;
            rbPrependXmlYes.Checked = workflowTransitionAction.PrependXml;
        }
        public void UpdateDataModel(InjectXmlWorkflowTransitionAction workflowTransitionAction)
        {
            workflowTransitionAction.TargetFieldXpath = txtTargetXPath.Text;
            workflowTransitionAction.Xml = txtXml.Text;
            workflowTransitionAction.PrependXml = rbPrependXmlYes.Checked;
        }
    }
}