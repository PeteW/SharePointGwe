using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    public class FormEditorWorkflowTransitionActionItem
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string TargetFieldXpath { get; set; }

        [DataMember]
        public string NewValue { get; set; }        
    }
    [DataContract]
    public class FormEditorWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string TargetFieldXpath { get; set; }

        [DataMember]
        public string NewValue { get; set; }

        [DataMember]
        public List<FormEditorWorkflowTransitionActionItem> Items { get; set; }

        public FormEditorWorkflowTransitionAction(){ Items = new List<FormEditorWorkflowTransitionActionItem>(); }

        public void HandleCallback(object sender, GweActionConfigUserControlCallbackEventArgs args)
        {
            var uc = sender as IWorkflowTransitionActionConfigControl<FormEditorWorkflowTransitionAction>;
            sender.AssertNotNull("sender was null");
            args.AssertNotNull("The arguments passed on the callback were null.");
            uc.AssertNotNull("the sender was not IWorkflowTransitionActionConfigControl<FormEditorWorkflowTransitionAction>");
            uc.UpdateDataModel(this);
            if (args.CommandName == "AddItem")
            {
                var item = new FormEditorWorkflowTransitionActionItem();
                item.Id = Guid.NewGuid();
                Items.Add(item);
            }
            else if(args.CommandName=="DeleteItem")
            {
                args.CommandArgs.AssertNotNull("The commandargs were null");
                var id = ((Guid) args.CommandArgs);
                Items = Items.Where(x => x.Id.ToString().ToLower() != id.ToString().ToLower()).ToList();
            }
            else
                throw new Exception(string.Format("FormEditorWorkflowTransitionActionItem: unsupported CommandName: [{0}]", args.CommandName));
            if(OnSaveRequested==null)
                throw new Exception("Unable to call Save() fom the action to the UI.");
            uc.UpdateUi(this);
            OnSaveRequested(this, EventArgs.Empty);
        }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<FormEditorWorkflowTransitionAction>) page.LoadControl("~/_controltemplates/Oxbow.Gwe/FormEditorWorkflowTransitionActionConfig.ascx");
            //handle callbacks from the user control
            ((IWorkflowTransitionActionConfigControlCallback) result).Callback = HandleCallback;
            result.UpdateUi(this);
            return (UserControl) result;
        }

        public override void UpdateFromUserControl(UserControl userControl)
        {
            ((IWorkflowTransitionActionConfigControl<FormEditorWorkflowTransitionAction>) userControl).UpdateDataModel(this);
        }
        
        public override string GetTypeName() { return "Form Editor"; }

        public override void Execute(SPListItem spListItem)
        {
            var formContainer = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration) as IFormContainer;
            if(formContainer==null)
                throw new Exception(spListItem.GetDescription()+" : Expected to have a type IFormContainer.");
            try
            {
                foreach (var item in Items)
                {
                    var templateEngine = ResolveType.Instance.Of<ITemplateEngine>();
                    var value = templateEngine.Render(item.NewValue, formContainer);
                    var xpath = templateEngine.Render(item.TargetFieldXpath, formContainer);
                    formContainer.SetNodeByXpath(xpath, value);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Error with the field modifier [{0}]: {1}", TargetFieldXpath, exp));
            }
            try
            {
                Thread.Sleep(2000);
                formContainer.PerformSystemSave();
                Thread.Sleep(2000);
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("Error when saving the list item [{0}]: {1}", spListItem.Title, exp));                
            }
        }
    }
}