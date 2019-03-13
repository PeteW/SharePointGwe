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
    public partial class FormEditorWorkflowTransitionActionConfig : System.Web.UI.UserControl, IWorkflowTransitionActionConfigControl<FormEditorWorkflowTransitionAction>, IWorkflowTransitionActionConfigControlCallback
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnAddItem.Click += new EventHandler(btnAddItem_Click);
            rptItems.ItemCommand += new RepeaterCommandEventHandler(rptItems_ItemCommand);
        }

        void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Callback == null)
                throw new Exception("The callback was null and so we cannot send the event anywhere.");
            Callback(this, new GweActionConfigUserControlCallbackEventArgs()
                               {
                                   CommandName = "DeleteItem",
                                   CommandArgs = new Guid(e.CommandArgument.ToString()) 
                               });

        }

        void btnAddItem_Click(object sender, EventArgs e)
        {
            if(Callback==null)
                throw new Exception("The callback was null and so we cannot send the event anywhere.");
            Callback(this, new GweActionConfigUserControlCallbackEventArgs(){CommandName = "AddItem"});
        }

        public void UpdateUi(FormEditorWorkflowTransitionAction workflowTransitionAction)
        {
            //this is an upgrade from prior versions of the code that didnt support multiple mutators in a single action
            if(!string.IsNullOrEmpty(workflowTransitionAction.TargetFieldXpath))
            {
                var formEditorWorkflowTransitionActionItem = new FormEditorWorkflowTransitionActionItem();
                formEditorWorkflowTransitionActionItem.Id = Guid.NewGuid();
                formEditorWorkflowTransitionActionItem.TargetFieldXpath = workflowTransitionAction.TargetFieldXpath;
                formEditorWorkflowTransitionActionItem.NewValue = workflowTransitionAction.NewValue;
                workflowTransitionAction.Items.Add(formEditorWorkflowTransitionActionItem);
                workflowTransitionAction.NewValue = string.Empty;
                workflowTransitionAction.TargetFieldXpath = string.Empty;
            }
            rptItems.DataSource = workflowTransitionAction.Items;
            rptItems.DataBind();
        }
        public void UpdateDataModel(FormEditorWorkflowTransitionAction workflowTransitionAction) { 
            workflowTransitionAction.Items.Clear();
            foreach (RepeaterItem item in rptItems.Items)
            {
                var w = new FormEditorWorkflowTransitionActionItem();
                w.Id = new Guid(((HiddenField) item.FindControl("hidId")).Value);
                w.NewValue = ((TextBox)item.FindControl("txtNewValue")).Text;
                w.TargetFieldXpath = ((TextBox) item.FindControl("txtTargetXPath")).Text;
                workflowTransitionAction.Items.Add(w);
            }
            workflowTransitionAction.TargetFieldXpath = string.Empty;
            workflowTransitionAction.NewValue = string.Empty;
        }

        public EventHandler<GweActionConfigUserControlCallbackEventArgs> Callback { get; set; }
    }
}