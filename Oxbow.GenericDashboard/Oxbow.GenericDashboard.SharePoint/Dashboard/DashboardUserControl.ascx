<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardUserControl.ascx.cs" Inherits="Oxbow.GenericDashboard.SharePoint.Dashboard.DashboardUserControl" %>

<script type="text/javascript">
    $(document).ready(function() { 
        $(".dashboardJumpMenu").change(function(e){
            e.preventDefault();
            var url = $(this).val();
            var target = $(this).find("option:selected").attr("target");
            if(!url || url=="")
                return;
            $(this).val($(this).find("option:first").val());
            window.open(url, target);
        });
        var tabIndex = $.cookie("tab_" + window.location);
        if(!tabIndex)
            tabIndex = 0;
        $(".tabs").tabs({
            selected: tabIndex,
            select: function(event, ui) {
                $.cookie("tab_" + window.location, ui.index, { expires: 1 });
            }
        });
        $(".excelExportButton").button({ icons: { primary: "ui-icon-calculator"} });
<%--         $("#<%=lnkExcelExport.ClientID %>").button({ icons: { primary: "ui-icon-calculator"} });--%>
        $(".ddCustomActions").change(function() {
            $("#<%=hidSelectedTabId.ClientID %>").val($(this).attr("tabid"));
            $("#<%=hidSelectedRowCustomActionId.ClientID %>").val($(this).find("option:selected").val());
            $("#<%=hidSelectedListItemId.ClientID %>").val($(this).attr("itemid"));
//            debugger;
            __doPostBack('<%=lnkRunCustomAction.UniqueID%>', '');
        });

    <asp:Repeater ID="rptTableConfigurations" runat="server">
    <ItemTemplate>
        <%#Eval("DataTablesDefinition")%>
    </ItemTemplate>
    </asp:Repeater>
        if(<%=OpenExcelWindow.ToString().ToLower() %>)
            window.open('/_layouts/Oxbow.GenericDashboard.SharePoint/ExcelExport.aspx?q=<%=HttpUtility.UrlEncode(GenericDashboardConfiguration.Dehydrate()) %>', "_blank");
    <asp:placeholder id="plExcelExport" runat="server" visible="false">
        
        </asp:placeHolder>
        });
    </script>
    <style type="text/css">
    .dashboardJumpMenu
    {
        float:left;
        padding-left: 20px;
}
.excelExportButton
{
    float:left;
}
.dvToolbar
{
    min-height:25px;
    display:block;
}
.dataTables_length
{
    display:inline;
    float:left;
}
.dataTables_filter
{
    display:inline;
    float:right;
}
</style>
<asp:Panel ID="pnlErrors" runat="server" Visible="false">
    <div class="ui-state-error ui-corner-all" style="padding: 0pt 0.7em;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em;"></span>
            <asp:Literal ID="ltrErrorMessage" runat="server" />
        </p>
    </div>
</asp:Panel>
<asp:Panel ID="pnlHighlights" runat="server" Visible="false">
    <div class="ui-state-highlight ui-corner-all" style="padding: 0pt 0.7em;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em;"></span>
            <asp:Literal ID="ltrHighlightMessage" runat="server" />
        </p>
    </div>
</asp:Panel>

<div class="dvToolbar ui-helper-clearfix">
    <a class="excelExportButton" href="/_layouts/Oxbow.GenericDashboard.SharePoint/ExcelExport.aspx?q=<%=HttpUtility.UrlEncode(GenericDashboardConfiguration.Dehydrate()) %>" target="_blank">Export to Excel</a>
<%--<asp:LinkButton ID="lnkExcelExport" CssClass="excelExportButton" runat="server" Text="Export to Excel" />--%>
    <asp:LinkButton runat="server" ID="lnkRunCustomAction"/>
    <asp:HiddenField runat="server" ID="hidSelectedTabId"/>
    <asp:HiddenField runat="server" ID="hidSelectedListItemId"/>
    <asp:HiddenField runat="server" ID="hidSelectedRowCustomActionId"/>
<asp:Literal ID="ltrJumpMenu" runat="server" />
</div>
<div class="tabs">
    <ul>
        <asp:Repeater ID="rptTabs" runat="server">
            <ItemTemplate>
                <li><a href='#<%#Eval("TabId") %>'>
                    <%#Eval("TabCaption")%></a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <asp:Repeater ID="rptTables" runat="server">
        <ItemTemplate>
            <div id='<%#Eval("TabId") %>'>
                <%#Eval("TableHtml")%>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
