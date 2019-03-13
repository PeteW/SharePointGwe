using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Oxbow.GenericDashboard.Core.Models;
using Oxbow.GenericDashboard.Core.Utils;

namespace Oxbow.GenericDashboard.SharePoint.CONTROLTEMPLATES
{
    public partial class WebPartConfig : BaseUserControl
    {
        private string _configXml = null;
        public void SetConfigXml(string input)
        {
            _configXml = input;
            BindControls();
        }
        public string GetConfigXml()
        {
            ReloadDataFromUi();
            return GenericDashboardConfigurationInViewState.Dehydrate();
        }

        public bool ReloadDataFromUi()
        {
            try
            {
                var d = GenericDashboardConfigurationInViewState;
                d.JumpMenuPrompt = txtJumpMenuPrompt.Text;
                foreach (RepeaterItem ri in rptTabs.Items)
                {
                    var hidTabConfigurationId = ri.FindControl("hidTabConfigurationId") as HiddenField;
                    var hidTabConfigurationOrderId = ri.FindControl("hidTabConfigurationOrderId") as HiddenField;
                    var txtName = ri.FindControl("txtName") as TextBox;
                    var txtWebUrl = ri.FindControl("txtWebUrl") as TextBox;
                    var txtListName = ri.FindControl("txtListName") as TextBox;
                    var txtViewName = ri.FindControl("txtViewName") as TextBox;
                    var txtPageSize = ri.FindControl("txtPageSize") as TextBox;
                    var chkDisplayCount = ri.FindControl("chkDisplayCount") as CheckBox;
                    var rptRowCustomActions = ri.FindControl("rptRowCustomActions") as Repeater;

                    var tab = d.TabConfigurations.Single(x => x.Id == new Guid(hidTabConfigurationId.Value));
                    tab.Name = txtName.Text;
                    tab.OrderId = int.Parse(hidTabConfigurationOrderId.Value);
                    tab.WebUrl = txtWebUrl.Text;
                    tab.ListName = txtListName.Text;
                    tab.PageSize = int.Parse(txtPageSize.Text);
                    tab.ViewName = txtViewName.Text;
                    tab.IsCountDisplayed = chkDisplayCount.Checked;

                    foreach (RepeaterItem ric in rptRowCustomActions.Items)
                    {
                        var hidRowCustomActionId = ric.FindControl("hidRowCustomActionId") as HiddenField;
                        var hidRowCustomActionOrderId = ric.FindControl("hidRowCustomActionOrderId") as HiddenField;
                        var txtRowCustomActionName = ric.FindControl("txtRowCustomActionName") as TextBox;
                        var txtRowCustomAssemblyName = ric.FindControl("txtRowCustomAssemblyName") as TextBox;
                        var txtRowCustomTypeName = ric.FindControl("txtRowCustomTypeName") as TextBox;
                        var rowCustomActionId = new Guid(hidRowCustomActionId.Value);
                        
                        var rowCustomAction = tab.RowCustomActions.SingleOrError(x => x.Id == rowCustomActionId, string.Format("Unable to find single RowCustomAction with Id [{0}]", rowCustomActionId));
                        rowCustomAction.Name = txtRowCustomActionName.Text;
                        rowCustomAction.OrderId = int.Parse(hidRowCustomActionOrderId.Value);
                        rowCustomAction.AssemblyName = txtRowCustomAssemblyName.Text;
                        rowCustomAction.TypeName = txtRowCustomTypeName.Text;
                    }
                }
                foreach (RepeaterItem ri in rptJumpMenuItems.Items)
                {
                    var hidJumpMenuItemId = ri.FindControl("hidJumpMenuItemId") as HiddenField;
                    var hidJumpMenuItemOrderId = ri.FindControl("hidJumpMenuItemOrderId") as HiddenField;
                    var txtName = ri.FindControl("txtName") as TextBox;
                    var txtUrl = ri.FindControl("txtUrl") as TextBox;
                    var txtTarget = ri.FindControl("txtTarget") as TextBox;

                    var jumpMenu = d.JumpMenuItems.Single(x => x.Id == new Guid(hidJumpMenuItemId.Value));
                    jumpMenu.OrderId = int.Parse(hidJumpMenuItemOrderId.Value);
                    jumpMenu.Name = txtName.Text;
                    jumpMenu.Target = txtTarget.Text;
                    jumpMenu.Url = txtUrl.Text;
                }
                GenericDashboardConfigurationInViewState = d;
                return true;
            }
            catch (Exception e)
            {
                DisplayErrorMessage(e);
                return false;
            }
        }

        protected override void WireEvents()
        {
            lnkAddJumpMenuItem.Click += new EventHandler(lnkAddJumpMenuItem_Click);
            lnkAddTab.Click += new EventHandler(lnkAddTab_Click);
            btnImport.Click += new EventHandler(btnImport_Click);
            rptTabs.ItemDataBound += new RepeaterItemEventHandler(rptTabs_ItemDataBound);
        }

        void rptTabs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var tab = (TabConfiguration) e.Item.DataItem;
            var rptRowCustomActions = e.Item.FindControl("rptRowCustomActions") as Repeater;
            rptRowCustomActions.DataSource = tab.RowCustomActions.OrderBy(x => x.OrderId).ToList();
            rptRowCustomActions.DataBind();
        }

        void btnImport_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                GenericDashboardConfigurationInViewState = GenericDashboardConfiguration.Hydrate(txtImportExport.Text);
                BindControls();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        void lnkAddTab_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                if (!ReloadDataFromUi())
                    return;
                var g = GenericDashboardConfigurationInViewState;
                g.AddTabConfiguration();
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        void lnkAddJumpMenuItem_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                if (!ReloadDataFromUi())
                    return;
                var g = GenericDashboardConfigurationInViewState;
                g.AddJumpMenuItem();
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

        public GenericDashboardConfiguration GenericDashboardConfigurationInViewState
        {
            get
            {
                if (ViewState["GenericDashboardConfiguration"] == null)
                {
                    GenericDashboardConfiguration c;
                    if (string.IsNullOrEmpty(_configXml))
                        c = new GenericDashboardConfiguration();
                    else
                        c = GenericDashboardConfiguration.Hydrate(_configXml);
                    ViewState["GenericDashboardConfiguration"] = c;
                }
                return (GenericDashboardConfiguration)ViewState["GenericDashboardConfiguration"];
            }
            set
            {
                ViewState["GenericDashboardConfiguration"] = value;
            }
        }

        protected override void CustomOnLoad()
        {
            txtImportExport.Text = GenericDashboardConfigurationInViewState.Dehydrate();
        }

        protected override void BindControls()
        {
            txtJumpMenuPrompt.Text = GenericDashboardConfigurationInViewState.JumpMenuPrompt;
            rptJumpMenuItems.DataSource = GenericDashboardConfigurationInViewState.JumpMenuItems.OrderBy(x => x.OrderId);
            rptJumpMenuItems.DataBind();
            rptTabs.DataSource = GenericDashboardConfigurationInViewState.TabConfigurations.OrderBy(x => x.OrderId);
            rptTabs.DataBind();
        }

        protected void rptRowCustomActions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!ReloadDataFromUi())
                return;
            var id = new Guid(e.CommandArgument.ToString());
            var tabId = new Guid(((HiddenField) e.Item.FindControl("hidTabId")).Value);
            var g = GenericDashboardConfigurationInViewState;
            var tab = g.TabConfigurations.SingleOrError(x => x.Id == tabId,string.Format("Unable to find single TabConfiguration with Id [{0}]", tabId));
            if (e.CommandName == "Delete")
            {
                tab.RemoveRowCustomAction(id);
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            else
                throw new Exception(string.Format("Unknown command :[{0}]", e.CommandName));
        }

        protected void rptTabs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!ReloadDataFromUi())
                return;
            var tabId = new Guid(e.CommandArgument.ToString());
            if (e.CommandName == "Delete")
            {
                var g = GenericDashboardConfigurationInViewState;
                g.RemoveTabConfiguration(tabId);
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            else if (e.CommandName == "AddRowCustomAction")
            {
                var g = GenericDashboardConfigurationInViewState;
                var tab = g.TabConfigurations.SingleOrError(x => x.Id == tabId, string.Format("Unable to find single TabConfiguration with Id [{0}]", tabId));
                tab.AddRowCustomAction();
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            else
                throw new Exception(string.Format("Unknown command :[{0}]", e.CommandName));
        }

        protected void rptJumpMenuItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!ReloadDataFromUi())
                return;
            var id = new Guid(e.CommandArgument.ToString());
            if (e.CommandName == "Delete")
            {
                var g = GenericDashboardConfigurationInViewState;
                g.RemoveJumpMenuItem(id);
                GenericDashboardConfigurationInViewState = g;
                BindControls();
            }
            else
                throw new Exception(string.Format("Unknown command :[{0}]", e.CommandName));
        }
    }
}