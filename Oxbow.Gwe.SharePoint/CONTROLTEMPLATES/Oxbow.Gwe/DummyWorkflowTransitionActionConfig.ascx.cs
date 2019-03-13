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
    public partial class DummyWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<DummyWorkflowTransitionAction>
    {
        public void UpdateUi(DummyWorkflowTransitionAction dummyWorkflowTransitionAction) { txtProperty.Text = dummyWorkflowTransitionAction.DummyProperty; }
        public void UpdateDataModel(DummyWorkflowTransitionAction dummyWorkflowTransitionAction){dummyWorkflowTransitionAction.DummyProperty = txtProperty.Text;}
    }
}