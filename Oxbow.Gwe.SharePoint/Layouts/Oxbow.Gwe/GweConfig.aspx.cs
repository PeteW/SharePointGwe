using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Core.WorkflowEngine;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.SharePoint
{
    public partial class GweConfig : Page
    {
        private SPList _currentList;
        private SPWeb _currentWeb;
        private WorkflowConfiguration _workflowConfiguration;

        public List<string> ExpandedTransitionIds
        {
            get
            {
                if (ViewState["ExpandedTransitionIds"] == null)
                    ViewState["ExpandedTransitionIds"] = new List<string>();
                return (List<string>) ViewState["ExpandedTransitionIds"];
            }
            set { ViewState["ExpandedTransitionIds"] = value; }
        }

        protected SPList CurrentList
        {
            get
            {
                if (_currentList == null)
                    _currentList = CurrentWeb.Lists[new Guid(Request.QueryString["List"])];
                return _currentList;
            }
        }

        protected SPWeb CurrentWeb
        {
            get
            {
                if (_currentWeb == null)
                {
                    _currentWeb = SPContext.Current.Web;
                }
                return _currentWeb;
            }
        }

        protected WorkflowConfiguration WorkflowConfiguration
        {
            get
            {
                if (_workflowConfiguration == null)
                {
                    var repo = new WorkflowConfigurationRepository(CurrentWeb);
                    _workflowConfiguration = repo.GetWorkflowConfigurationByListName(CurrentList.Title);
                    if (_workflowConfiguration == null)
                        _workflowConfiguration = new WorkflowConfiguration();
                }
                return _workflowConfiguration;
            }
        }

        public bool IsActionContainerVisible(WorkflowTransition workflowTransition) { return ExpandedTransitionIds.Any(x => workflowTransition.Id.ToString() == x); }

        public static string GetIconClass(AgentLogItemSeverity severity)
        {
            if (severity == AgentLogItemSeverity.Debug)
                return "ui-icon-check";
            if (severity == AgentLogItemSeverity.Info)
                return "ui-icon-info";
            if (severity == AgentLogItemSeverity.Warn)
                return "ui-icon-notice";
            if (severity == AgentLogItemSeverity.Error)
                return "ui-icon-alert";
            return string.Empty;
        }

        private void btnRunTimeTrigger_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                pnlTimerResponse.Visible = true;
                IList<AgentLogItem> logs = GweAgentRunner.RunJob(CurrentList, TimeTriggerJobRunner.RunTimerJob);
                rptTimerLogs.DataSource = logs;
                rptTimerLogs.DataBind();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }


        public void BindControls()
        {
            bool isRegistered = false;
            CurrentWeb.RunUnsafeWithElevatedPrivileges(w => isRegistered = EventReceiverUtils.IsEventReceiverRegistered(w.Lists[CurrentList.Title]));
            lnkBack.Text = "Back to " + CurrentList.Title;
            lnkBack.NavigateUrl = CurrentList.DefaultViewUrl;
            chkRegisterEventReceiver.Checked = isRegistered;
            txtNextActionXpath.Text = WorkflowConfiguration.SelectedActionExpression;
            txtAdminToEmail.Text = WorkflowConfiguration.AdminToEmail;
            txtAdminFromEmail.Text = WorkflowConfiguration.AdminFromEmail;
            txtAdminCcEmail.Text = WorkflowConfiguration.AdminCcEmail;
            rptTimeTriggers.DataSource = WorkflowConfiguration.WorkflowTimeTriggers.OrderBy(x => x.OrderId);
            rptTimeTriggers.DataBind();
            rptWorkflowConfigurationVariables.DataSource = WorkflowConfiguration.WorkflowConfigurationVariables;
            rptWorkflowConfigurationVariables.DataBind();
            rptTransitions.DataSource = WorkflowConfiguration.WorkflowTransitions;
            rptTransitions.DataBind();
            ltrXml.Text = WorkflowConfiguration.SerializeToString();
        }

        private void btnAddTimeTrigger_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                WorkflowConfiguration.AddWorkflowTimeTrigger(new WorkflowTimeTrigger {Name = "TimeTrigger_" + (WorkflowConfiguration.WorkflowTimeTriggers.Count() + 1)});
                Save();
                DisplayFeedback("Time trigger added.");
            }
            catch (Exception x)
            {
                DisplayErrorMessage(x);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnRunTimeTrigger.Click += btnRunTimeTrigger_Click;
            btnAddTimeTrigger.Click += btnAddTimeTrigger_Click;
            btnImportXml.Click += new EventHandler(btnImportXml_Click);
            btnSave.Click += btnSave_Click;
            rptTransitions.ItemCommand += rptTransitions_ItemCommand;
            rptTransitions.ItemDataBound += rptTransitions_ItemDataBound;
            btnAddTransition.Click += btnAddTransition_Click;
            chkRegisterEventReceiver.CheckedChanged += chkRegisterEventReceiver_CheckedChanged;
            rptTimeTriggers.ItemCommand += new RepeaterCommandEventHandler(rptTimeTriggers_ItemCommand);
            rptWorkflowConfigurationVariables.ItemCommand += new RepeaterCommandEventHandler(rptWorkflowConfigurationVariables_ItemCommand);
            btnAddWorkflowConfigurationVariable.Click += new EventHandler(btnAddWorkflowConfigurationVariable_Click);
            if (!IsPostBack)
                BindControls();
            else
                RebuildDynamicControlsAfterPostback();
        }

        void btnAddWorkflowConfigurationVariable_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                WorkflowConfiguration.AddWorkflowConfigurationVariable(new WorkflowConfigurationVariable{ Name = "Var_" + (WorkflowConfiguration.WorkflowConfigurationVariables.Length + 1)});
                Save();
                DisplayFeedback("Variable added.");
                BindControls();
            }
            catch (Exception x)
            {
                DisplayErrorMessage(x);
            }
        }

        void rptWorkflowConfigurationVariables_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                var workflowConfigurationVariableId = e.CommandArgument.ToString();
                if (e.CommandName == "Delete")
                {
                    WorkflowConfiguration.RemoveWorkflowConfigurationVariable(workflowConfigurationVariableId);
                    Save();
                    DisplayFeedback("Variable removed.");
                }
                else
                {
                    throw new Exception("Unknown commandargument:" + e.CommandArgument);
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        void btnImportXml_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                var workflowConfiguration = WorkflowConfiguration.DeserializeFromString(ltrXml.Text);
                new WorkflowConfigurationRepository(CurrentWeb).Save(CurrentList.Title,workflowConfiguration);
                DisplayFeedback("Workflow configuration imported.");
                _workflowConfiguration = workflowConfiguration;
                BindControls();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        void rptTimeTriggers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                var timeTriggerId = e.CommandArgument.ToString();
                if (e.CommandName == "Delete")
                {
                    WorkflowConfiguration.RemoveWorkflowTimeTrigger(timeTriggerId);
                    Save();
                    DisplayFeedback("Time trigger removed.");
                }
                else
                {
                    throw new Exception("Unknown commandargument:" + e.CommandArgument);
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }


        private void chkRegisterEventReceiver_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkRegisterEventReceiver.Checked)
                    CurrentWeb.RunUnsafeWithElevatedPrivileges(w => EventReceiverUtils.Register(w.Lists[CurrentList.Title]));
                else
                    CurrentWeb.RunUnsafeWithElevatedPrivileges(w => EventReceiverUtils.Unregister(w.Lists[CurrentList.Title]));
                DisplayFeedback("Event settings updated.");
                BindControls();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        private void btnAddTransition_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                WorkflowConfiguration.AddTransition(new WorkflowTransition {Name = "Transition_" + (WorkflowConfiguration.WorkflowTransitions.Length + 1)});
                Save();
                DisplayFeedback("Transition added.");
                BindControls();
            }
            catch (Exception x)
            {
                DisplayErrorMessage(x);
            }
        }

        private void RebuildDynamicControlsAfterPostback()
        {
            foreach (RepeaterItem t in rptTransitions.Items)
            {
                var hidTransitionName = t.FindControl("hidTransitionName") as HiddenField;
                var hidTransitionId = t.FindControl("hidTransitionId") as HiddenField;
                var rptActions = t.FindControl("rptActions") as Repeater;
                WorkflowTransition workflowTransition = WorkflowConfiguration.WorkflowTransitions.Where(x => x.Id.ToString() == hidTransitionId.Value).FirstOrDefault();
                foreach (RepeaterItem a in rptActions.Items)
                {
                    var hidWorkflowTransitionActionElementId = a.FindControl("hidWorkflowTransitionActionElementId") as HiddenField;
                    WorkflowTransitionActionElement workflowTransitionActionElement = workflowTransition.WorkflowTransitionActionElements.Where(x => x.Id.ToString() == hidWorkflowTransitionActionElementId.Value).FirstOrDefault();
                    var pnlWorkflowTransitionActionContainer = a.FindControl("pnlWorkflowTransitionActionContainer") as Panel;
                    WorkflowTransitionAction workflowTransitionAction = WorkflowTransitionActionFactory.GetWorkflowTransitionAction(workflowTransitionActionElement, WorkflowConfiguration);
                    UserControl userControl = workflowTransitionAction.GenerateUserControl(this);
                    workflowTransitionAction.OnSaveRequested = btnSave_Click;
                    userControl.ID = "uc";
                    pnlWorkflowTransitionActionContainer.Controls.Add(userControl);
                }
            }
        }

        private void rptTransitions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var workflowTransition = e.Item.DataItem as WorkflowTransition;
            var ddWorkflowTransitionActionTypes = e.Item.FindControl("ddWorkflowTransitionActionTypes") as DropDownList;
            var rptActions = e.Item.FindControl("rptActions") as Repeater;

            ddWorkflowTransitionActionTypes.DataSource = WorkflowTransitionActionFactory.AvailableTypes;
            ddWorkflowTransitionActionTypes.DataTextField = "Key";
            ddWorkflowTransitionActionTypes.DataValueField = "Value";
            ddWorkflowTransitionActionTypes.DataBind();
            rptActions.DataSource = workflowTransition.WorkflowTransitionActionElements.OrderBy(x => x.OrderId);
            rptActions.DataBind();
        }

        protected void rptActions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ltrWorkflowTransitionActionElementHeader = e.Item.FindControl("ltrWorkflowTransitionActionElementHeader") as Literal;
            var workflowTransitionActionElement = e.Item.DataItem as WorkflowTransitionActionElement;
            var hidWorkflowTransitionActionElementId = e.Item.FindControl("hidWorkflowTransitionActionElementId") as HiddenField;
            var hidWorkflowTransitionActionElementName = e.Item.FindControl("hidWorkflowTransitionActionElementName") as HiddenField;
            var hidWorkflowTransitionActionElementOrderId = e.Item.FindControl("hidWorkflowTransitionActionElementOrderId") as HiddenField;
            var pnlWorkflowTransitionActionContainer = e.Item.FindControl("pnlWorkflowTransitionActionContainer") as Panel;
            hidWorkflowTransitionActionElementOrderId.Value = e.Item.ItemIndex.ToString();
            WorkflowTransitionAction workflowTransitionAction = WorkflowTransitionActionFactory.GetWorkflowTransitionAction(workflowTransitionActionElement, WorkflowConfiguration);
            ltrWorkflowTransitionActionElementHeader.Text = workflowTransitionAction.GetTypeName();
            UserControl userControl = workflowTransitionAction.GenerateUserControl(this);
            hidWorkflowTransitionActionElementName.Value = workflowTransitionActionElement.Name;
            hidWorkflowTransitionActionElementId.Value = workflowTransitionActionElement.Id.ToString();
            userControl.ID = "uc";
            pnlWorkflowTransitionActionContainer.Controls.Add(userControl);
        }

        protected void rptActions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ClearMessages();
            if(string.IsNullOrEmpty((e.CommandArgument??"").ToString()) && string.IsNullOrEmpty((e.CommandName??"").ToString()))
                return;
            try
            {
                UpdateWorkflowConfigurationFromUi();
                string workflowTransitionActionElementId = e.CommandArgument.ToString();
                string transitionId = ((HiddenField) e.Item.Parent.Parent.FindControl("hidTransitionId")).Value;
                WorkflowTransition transition = WorkflowConfiguration.WorkflowTransitions.Where(x => x.Id.ToString() == transitionId).First();
                if (e.CommandName == "Delete")
                {
                    transition.RemoveWorkflowTransitionActionElement(workflowTransitionActionElementId);
                    Save();
                    DisplayFeedback("Action removed.");
                }
                else
                {
                    throw new Exception("Unknown commandargument:" + e.CommandArgument);
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        private void rptTransitions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                string transitionId = e.CommandArgument.ToString();
                WorkflowTransition workflowTransition = WorkflowConfiguration.WorkflowTransitions.Where(x => x.Id.ToString() == transitionId).FirstOrDefault();
                if (workflowTransition == null)
                    throw new Exception("Please save after adding a new transition");
                if (e.CommandName == "AddAction")
                {
                    var ddWorkflowTransitionActionTypes = e.Item.FindControl("ddWorkflowTransitionActionTypes") as DropDownList;
                    string[] split = ddWorkflowTransitionActionTypes.SelectedValue.Split(new[] {'|'});
                    var workflowTransitionActionElement = new WorkflowTransitionActionElement();
                    workflowTransitionActionElement.AssemblyName = split[1];
                    workflowTransitionActionElement.TypeName = split[0];
                    workflowTransitionActionElement.OrderId = workflowTransition.WorkflowTransitionActionElements.Count();
                    WorkflowTransitionAction workflowTransitionAction = WorkflowTransitionActionFactory.CreateNewWorkflowTransitionAction(workflowTransitionActionElement.TypeName, workflowTransitionActionElement.AssemblyName);
                    workflowTransitionActionElement.Name = workflowTransitionAction.GetTypeName() + "_" + ((workflowTransition.WorkflowTransitionActionElements.Length + 1));
                    workflowTransitionActionElement.ConfigXml = workflowTransitionAction.Serialize();
                    workflowTransition.AddWorkflowTransitionActionElement(workflowTransitionActionElement);
                    Save();
                }
                else if (e.CommandName == "Delete")
                {
                    WorkflowConfiguration.RemoveTransition(transitionId);
                    Save();
                    DisplayFeedback("Transition removed.");
                }
                else
                {
                    throw new Exception("Unknown commandargument:" + e.CommandArgument);
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                UpdateWorkflowConfigurationFromUi();
                Save();
                DisplayFeedback("Workflow settings saved.");
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        private void UpdateWorkflowConfigurationFromUi()
        {
            WorkflowConfiguration.SelectedActionExpression = txtNextActionXpath.Text;
            WorkflowConfiguration.AdminToEmail = txtAdminToEmail.Text;
            WorkflowConfiguration.AdminFromEmail = txtAdminFromEmail.Text;
            WorkflowConfiguration.AdminCcEmail = txtAdminCcEmail.Text;
            foreach (RepeaterItem workflowTransitionRepeaterItem in rptTransitions.Items)
            {
                var hidTransitionName = workflowTransitionRepeaterItem.FindControl("hidTransitionName") as HiddenField;
                var hidTransitionId = workflowTransitionRepeaterItem.FindControl("hidTransitionId") as HiddenField;
                var txtTransitionName = workflowTransitionRepeaterItem.FindControl("txtTransitionName") as TextBox;
                var rptActions = workflowTransitionRepeaterItem.FindControl("rptActions") as Repeater;
                WorkflowTransition workflowTransition = WorkflowConfiguration.WorkflowTransitions.Where(x => x.Id.ToString() == hidTransitionId.Value).FirstOrDefault();
                if (workflowTransition == null)
                    throw new Exception(string.Format("Unable to find the WorkflowTransition with Id [{0}]", hidTransitionId.Value));
                workflowTransition.Name = txtTransitionName.Text;
                foreach (RepeaterItem variableItem in rptWorkflowConfigurationVariables.Items)
                {
                    var hidWorkflowConfigurationVariableId = variableItem.FindControl("hidWorkflowConfigurationVariableId") as HiddenField;
                    var txtWorkflowConfigurationVariableName = variableItem.FindControl("txtWorkflowConfigurationVariableName") as TextBox;
                    var txtWorkflowConfigurationVariableValue = variableItem.FindControl("txtWorkflowConfigurationVariableValue") as TextBox;
                    var workflowConfigurationVariable = WorkflowConfiguration.WorkflowConfigurationVariables.Where(x => x.Id.ToString() == hidWorkflowConfigurationVariableId.Value).FirstOrDefault();
                    if(workflowConfigurationVariable==null)
                        throw new Exception(string.Format("Unable to find the WorkflowConfigurationVariable definition with ID [{0}]", hidWorkflowConfigurationVariableId.Value));
                    workflowConfigurationVariable.Name = txtWorkflowConfigurationVariableName.Text;
                    workflowConfigurationVariable.Value = txtWorkflowConfigurationVariableValue.Text;
                }
                foreach (RepeaterItem timeTriggerItem in rptTimeTriggers.Items)
                {
                    var hidTriggerId = timeTriggerItem.FindControl("hidTriggerId") as HiddenField;
                    var hidTriggerOrderId = timeTriggerItem.FindControl("hidTriggerOrderId") as HiddenField;
                    var txtTriggerName = timeTriggerItem.FindControl("txtTriggerName") as TextBox;
                    var txtViewName = timeTriggerItem.FindControl("txtViewName") as TextBox;
//                    var txtTimeTriggerExpression = timeTriggerItem.FindControl("txtTimeTriggerExpression") as TextBox;
                    var txtTransitionToExecute = timeTriggerItem.FindControl("txtTransitionToExecute") as TextBox;
                    var workflowTimeTrigger = WorkflowConfiguration.WorkflowTimeTriggers.Where(x => x.Id.ToString() == hidTriggerId.Value).FirstOrDefault();
                    if (workflowTimeTrigger == null)
                        throw new Exception(string.Format("Unable to find the time trigger with ID [{0}]", hidTriggerId));
                    workflowTimeTrigger.Name = txtTriggerName.Text;
                    workflowTimeTrigger.ViewName = txtViewName.Text;
                    workflowTimeTrigger.OrderId = int.Parse(hidTriggerOrderId.Value);
                    workflowTimeTrigger.ViewName = txtViewName.Text;
                    workflowTimeTrigger.TransitionToExecute = txtTransitionToExecute.Text;
                }
                foreach (RepeaterItem workflowTransitionActionRepeaterItem in rptActions.Items)
                {
                    var pnlWorkflowTransitionActionContainer = workflowTransitionActionRepeaterItem.FindControl("pnlWorkflowTransitionActionContainer") as Panel;
                    var hidWorkflowTransitionActionElementOrderId = workflowTransitionActionRepeaterItem.FindControl("hidWorkflowTransitionActionElementOrderId") as HiddenField;
                    var hidWorkflowTransitionActionElementId = workflowTransitionActionRepeaterItem.FindControl("hidWorkflowTransitionActionElementId") as HiddenField;
                    var hidWorkflowTransitionActionElementName = workflowTransitionActionRepeaterItem.FindControl("hidWorkflowTransitionActionElementName") as HiddenField;
                    var txtWorkflowTransitionActionElementName = workflowTransitionActionRepeaterItem.FindControl("txtWorkflowTransitionActionElementName") as TextBox;
                    var txtWorkflowTransitionActionElementExecuteConditionExpression = workflowTransitionActionRepeaterItem.FindControl("txtWorkflowTransitionActionElementExecuteConditionExpression") as TextBox;
                    var chkWorkflowTransitionActionHaltOnFailure = workflowTransitionActionRepeaterItem.FindControl("chkWorkflowTransitionActionHaltOnFailure") as CheckBox;

                    WorkflowTransitionActionElement workflowTransitionActionElement = workflowTransition.WorkflowTransitionActionElements.Where(x => x.Id.ToString() == hidWorkflowTransitionActionElementId.Value).FirstOrDefault();
                    workflowTransitionActionElement.Name = txtWorkflowTransitionActionElementName.Text;
                    workflowTransitionActionElement.ExecuteConditionExpression = txtWorkflowTransitionActionElementExecuteConditionExpression.Text;
                    workflowTransitionActionElement.HaltOnFailure = chkWorkflowTransitionActionHaltOnFailure.Checked;

                    var userControl = pnlWorkflowTransitionActionContainer.FindControl("uc") as UserControl;
                    if (workflowTransitionActionElement == null)
                        throw new Exception(string.Format("Unable to find the WorkflowTransitionActionElement named [{0}] within the WorkflowTransition named [{1}]", hidWorkflowTransitionActionElementName.Value, workflowTransition.Name));
                    WorkflowTransitionAction workflowTransitionAction = WorkflowTransitionActionFactory.GetWorkflowTransitionAction(workflowTransitionActionElement, WorkflowConfiguration);
                    workflowTransitionAction.UpdateFromUserControl(userControl);
                    workflowTransitionActionElement.ConfigXml = workflowTransitionAction.Serialize();
                    workflowTransitionActionElement.OrderId = int.Parse(hidWorkflowTransitionActionElementOrderId.Value);
                }
            }
        }

        private void Save()
        {
            new WorkflowConfigurationRepository(CurrentWeb).Save(CurrentList.Title, WorkflowConfiguration);
//            _workflowConfiguration = _workflowConfigurationStatic = WorkflowConfiguration;
            BindControls();
        }

        protected void DisplayErrorMessage(string message)
        {
            pnlErrors.Visible = true;
            ltrErrorMessage.Text = message;
        }

        protected void ClearMessages()
        {
            pnlErrors.Visible = false;
            ltrErrorMessage.Text = string.Empty;
            pnlHighlights.Visible = false;
            ltrHighlightMessage.Text = string.Empty;
        }

        protected void DisplayErrorMessage(Exception e)
        {
            //if debug mode is on them include the stack trace in what is shown to the user
//            if (SettingsManager.IsDebugMode)
                DisplayErrorMessage(e.ToString());
//            else
//                DisplayErrorMessage(e.Message);
        }

        protected void DisplayFeedback(string message)
        {
            pnlHighlights.Visible = true;
            ltrHighlightMessage.Text = message;
        }
    }
}