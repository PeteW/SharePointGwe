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
    public partial class CustomCodeWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<CustomCodeWorkflowTransitionAction>
    {
        public void UpdateUi(CustomCodeWorkflowTransitionAction workflowTransitionAction)
        {
            txtTypeName.Text = workflowTransitionAction.TypeName;
            txtAssemblyName.Text = workflowTransitionAction.AssemblyName;
        }
        public void UpdateDataModel(CustomCodeWorkflowTransitionAction workflowTransitionAction)
        {
            workflowTransitionAction.TypeName = txtTypeName.Text;
            workflowTransitionAction.AssemblyName = txtAssemblyName.Text;
        }

    }
}