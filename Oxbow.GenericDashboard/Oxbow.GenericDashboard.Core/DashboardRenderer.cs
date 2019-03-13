using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oxbow.GenericDashboard.Core.Configuration;
using Oxbow.GenericDashboard.Core.Models;
using Oxbow.GenericDashboard.Core.Utils;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
namespace Oxbow.GenericDashboard.Core
{
    [Serializable]
    public class DashboardView
    {
        public DashboardView() { DashboardTabs = new List<DashboardTabView>(); }
        public List<DashboardTabView> DashboardTabs { get; set; }
        public string JumpMenuHtml { get; set; }
    }
    [Serializable]
    public class DashboardTabView
    {
        public string TabId { get; set; }
        public string TabCaption { get; set; }
        public string TableHtml { get; set; }
        public string DataTablesDefinition { get; set; }
    }

    public class DashboardRenderer
    {
        public DashboardView Render(GenericDashboardConfiguration genericDashboardConfiguration, SPWeb webContext)
        {
            var result = new DashboardView();
            foreach (var tabConfiguration in genericDashboardConfiguration.TabConfigurations.OrderBy(x => x.OrderId))
            {
                result.DashboardTabs.Add(RenderTab(tabConfiguration, webContext));
            }
            result.JumpMenuHtml = RenderJumpMenu(genericDashboardConfiguration);
            return result;
        }
        public string RenderJumpMenu(GenericDashboardConfiguration genericDashboardConfiguration)
        {
            if (genericDashboardConfiguration.JumpMenuItems.Count == 0)
                return string.Empty;
            var template = @"
<select class='dashboardJumpMenu'>
<option>{1}</option>    
{0}
</select>";
            var sb = new StringBuilder();
            foreach (var jumpMenuItem in genericDashboardConfiguration.JumpMenuItems.OrderBy(x => x.OrderId))
            {
                sb.AppendFormat("<option value='{0}' target='{2}'>{1}</option>", jumpMenuItem.Url, jumpMenuItem.Name, jumpMenuItem.Target);
            }
            return string.Format(template, sb, genericDashboardConfiguration.JumpMenuPrompt);
        }

        public DashboardTabView RenderTab(TabConfiguration tabConfiguration, SPWeb webContext)
        {
            var result = new DashboardTabView();
            var count = 0;
            result.TabCaption = tabConfiguration.Name;
            result.TabId = tabConfiguration.Id.ToString();
            tabConfiguration.WebUrl = string.IsNullOrEmpty(tabConfiguration.WebUrl) ? webContext.Url : tabConfiguration.WebUrl;
            SharePointExtensions.Run(tabConfiguration.WebUrl, x =>
            {
                var list = x.GetSPListByName(tabConfiguration.ListName);
                var view = list.GetSPViewByName(tabConfiguration.ViewName);
                var listItems = list.GetItems(new SPQuery(view));
                var listwholeItems = list.GetItems(new SPQuery());
                count = listItems.Count;
                result.TableHtml = RenderTable(tabConfiguration, view, listItems, listwholeItems);
                result.DataTablesDefinition = RenderDataTableDefinition(tabConfiguration, view, listItems);
            });
            if (tabConfiguration.IsCountDisplayed)
                result.TabCaption += " (" + count + ")";
            return result;
        }

        public string RenderDataTableDefinition(TabConfiguration tabConfiguration, SPView view, SPListItemCollection collection)
        {
            var template = @"
        $('#{0}').dataTable({{
            'bJQueryUI': true,
            'iDisplayLength': {1},
            'bStateSave': true,
            'aaSorting': [],
            'bFilter': true,
            'bPaginate': true,
            'bInfo': true//,
//            'aoColumns': [
                //{{'sWidth': '100px', 'sClass': 'rightAlign', 'sSortDataType': 'dom-text' }}, 
  //          ]
        }});";
            return string.Format(template, "dataTable_" + tabConfiguration.Id, tabConfiguration.PageSize);
        }

        public string RenderTable(TabConfiguration tabConfiguration, SPView view, SPListItemCollection collection, SPListItemCollection listwholeitems)
        {
            if (collection.Count == 0)
                return "<div class='noMatchesFound'>No matches found.</div>";
            var template = @"
<table class='dataTable' id='{2}'>
    <thead>
        <tr>
            {0}
        </tr>
    </thead>
    <tbody>
        {1}
    </tbody>
</table>";
            var tableHead = new StringBuilder();
            var tableBody = new StringBuilder();
            if (tabConfiguration.RowCustomActions.Count > 0)
            {
                tableHead.AppendFormat("<th>&nbsp;</th>");
            }
            if (collection.Count > 0)//added by vani on march202013
            {
                tableHead.AppendFormat("<th>OtherCartContents</th>");
                tableHead.AppendFormat("<th>SRMCart</th>");
            }

            for (var i = 0; i < view.ViewFields.Count; i++)
            {
                var internalFieldName = view.ViewFields[i];
                var spField = collection[0].Fields.GetFieldByInternalName(internalFieldName);
                tableHead.AppendFormat("<th>{0}</th>", spField.Title);
            }
            foreach (SPListItem spListItem in collection)
            {
                tableBody.Append(RenderTableRow(tabConfiguration, view, spListItem, listwholeitems));
            }
            return string.Format(template, tableHead, tableBody, "dataTable_" + tabConfiguration.Id);
        }

        public string RenderTableRow(TabConfiguration tabConfiguration, SPView view, SPListItem spListItem, SPListItemCollection collection)
        {
            var template = @"
<tr>
    {0}
</tr>";
            var rowBody = new StringBuilder();
            string strOtherItems = RenderOtherItems(tabConfiguration, collection);
            string strSrm = SrmCartStatus(spListItem);
            if (tabConfiguration.RowCustomActions.Count > 0)
            {
                rowBody.AppendFormat("<td>{0}</td>", RenderRowCustomActionDropDownList(tabConfiguration, spListItem));
                rowBody.AppendFormat("<td>{0}</td>", strOtherItems);//added by vani on march202013
                rowBody.AppendFormat("<td>{0}</td>", strSrm);//added by vani on march212013

            }
            for (var i = 0; i < view.ViewFields.Count; i++)
            {
                var internalFieldName = view.ViewFields[i];
                var spField = spListItem.Fields.GetFieldByInternalName(internalFieldName);
                var renderer = ResolveType.Instance.OfSpFieldRenderer(spField.Type);
                var value = renderer.RenderHtml(spListItem, spField, (i == 0));
                rowBody.AppendFormat("<td>{0}</td>", value);
            }
            return string.Format(template, rowBody);
        }

        public string RenderRowCustomActionDropDownList(TabConfiguration tabConfiguration, SPListItem spListItem)
        {
            var stringBuilder = new StringBuilder(string.Format("<select class='ddCustomActions' tabid='{0}' itemid='{1}'>", tabConfiguration.Id, spListItem.ID));
            stringBuilder.Append("<option value=''>Action...</option>");
            foreach (var rowCustomAction in tabConfiguration.RowCustomActions)
            {
                stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", rowCustomAction.Id, rowCustomAction.Name);
            }
            stringBuilder.Append("</select>");
            return stringBuilder.ToString();
        }
        //added by vani on march 21st march 2013
        public string SrmCartStatus(SPListItem spListItem)
        {

            var sb = new StringBuilder();

            if (spListItem["Status"] != null && !string.IsNullOrEmpty(spListItem["Status"].ToString()))
            {
                if (spListItem["Status"].ToString() == "ItamGcsFulfillmentApproval")
                {
                    var cartIdFieldName = "CartID";
                    var spField = spListItem.Fields.GetFieldByInternalName(cartIdFieldName);
                    var renderer = ResolveType.Instance.OfSpFieldRenderer(spField.Type);
                    var value = renderer.RenderHtml(spListItem, spField, false);
                    sb.AppendFormat("<input type='text' id='{0}'/>", "txtsrm" + value);
                    sb.AppendFormat("<input type='button' value='Save' id='{0}'/>", "btnsrm" + value);
                }
            }
            return sb.ToString();
        }
        //added by vani on march20th2013
        public string RenderOtherItems(TabConfiguration tabConfiguration, SPListItemCollection collection)
        {
            var sb = new StringBuilder();
            sb.Append("<select class='ddOtherItems'>");
            sb.Append("<option value=''>More Items...</option>");
            int i = default(int);
            //var orderList = GetSpList(SPContext.Current.Web, tabConfiguration.ListName);
            foreach (SPListItem item in collection)
            {

                var cartIdFieldName = "CartID"; //GetSpField(orderList, "CartID").InternalName;
                var statusFieldName = "Status";//GetSpField(orderList, "Status").InternalName;
                var productFieldName = "Product_x0020_Name";
                //var internalFieldName = "Status";
                var spField = item.Fields.GetFieldByInternalName(cartIdFieldName);
                var renderer = ResolveType.Instance.OfSpFieldRenderer(spField.Type);
                var value = renderer.RenderHtml(item, spField, (i == 0));

                var spField1 = item.Fields.GetFieldByInternalName(statusFieldName);
                var renderer1 = ResolveType.Instance.OfSpFieldRenderer(spField1.Type);
                var value1 = renderer1.RenderHtml(item, spField1, (i == 0));

                var spField2 = item.Fields.GetFieldByInternalName(productFieldName);
                var renderer2 = ResolveType.Instance.OfSpFieldRenderer(spField2.Type);
                var value2 = renderer2.RenderHtml(item, spField2, (i == 0));

                sb.AppendFormat("<option value=''>{0}</option>", value2 + "( " + value1 + " )");
                i += 1;
            }
            sb.Append("</select>");
            return sb.ToString();
        }
    }

}