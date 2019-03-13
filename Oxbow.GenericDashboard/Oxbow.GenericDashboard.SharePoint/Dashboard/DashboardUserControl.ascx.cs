using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Oxbow.GenericDashboard.Core;
using Oxbow.GenericDashboard.Core.Models;
using Oxbow.GenericDashboard.Core.Utils;

namespace Oxbow.GenericDashboard.SharePoint.Dashboard
{
    public partial class DashboardUserControl : BaseUserControl
    {
        public bool OpenExcelWindow { get; set; }
        public void SetConfigXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;
            GenericDashboardConfiguration = GenericDashboardConfiguration.Hydrate(input);
            DashboardView = new DashboardRenderer().Render(GenericDashboardConfiguration, SPContext.Current.Web);
            CustomOnLoad();
        }
        public GenericDashboardConfiguration GenericDashboardConfiguration
        {
            get { return (GenericDashboardConfiguration)ViewState["GenericDashboardConfiguration"] ?? new GenericDashboardConfiguration(); }
            set { ViewState["GenericDashboardConfiguration"] = value; }
        }

        public DashboardView DashboardView
        {
            get { return (DashboardView)ViewState["DashboardView"] ?? new DashboardView(); }
            set { ViewState["DashboardView"] = value; }
        }

        protected override void CustomOnLoad()
        {
            rptTabs.DataSource = DashboardView.DashboardTabs;
            rptTabs.DataBind();
            rptTableConfigurations.DataSource = DashboardView.DashboardTabs;
            rptTableConfigurations.DataBind();
            rptTables.DataSource = DashboardView.DashboardTabs;
            rptTables.DataBind();
            ltrJumpMenu.Text = DashboardView.JumpMenuHtml;
        }

        protected override void WireEvents()
        {
//            lnkExcelExport.Click += new EventHandler(lnkExcelExport_Click);
            lnkRunCustomAction.Click += new EventHandler(lnkRunCustomAction_Click);
        }

        void lnkRunCustomAction_Click(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                var g = GenericDashboardConfiguration;
                var tabId = new Guid(hidSelectedTabId.Value);
                var itemId = int.Parse(hidSelectedListItemId.Value);
                var actionId = new Guid(hidSelectedRowCustomActionId.Value);
                var tabConfiguration = g.TabConfigurations.SingleOrError(x => x.Id == tabId, string.Format("Unable to find the tab with Id [{0}]", tabId));
                var action = tabConfiguration.RowCustomActions.SingleOrError(x => x.Id == actionId, string.Format("Unable to find the action with Id [{0}]", actionId));
                
                tabConfiguration.WebUrl = string.IsNullOrEmpty(tabConfiguration.WebUrl) ? SPContext.Current.Web.Url : tabConfiguration.WebUrl;
                
                SharePointExtensions.Run(tabConfiguration.WebUrl, x =>
                {
                    var list = x.GetSPListByName(tabConfiguration.ListName);
                    var item = list.GetSpListItemById(itemId);
                    action.Execute(item,action.Name);
                    DisplayFeedback(string.Format("Successfully ran the action [{0}] on item #[{1}] from list [{2}]", action.Name, itemId, list.Title));
                    DashboardView = new DashboardRenderer().Render(GenericDashboardConfiguration, SPContext.Current.Web);
                    CustomOnLoad();
                });
            }
            catch(Exception exp)
            {
                DisplayErrorMessage(exp);
            }
        }

//        void lnkExcelExport_Click(object sender, EventArgs e)
//        {
//            Session["GenericDashboardConfiguration"] = GenericDashboardConfiguration;
//            OpenExcelWindow = true;
//        }
        }
}