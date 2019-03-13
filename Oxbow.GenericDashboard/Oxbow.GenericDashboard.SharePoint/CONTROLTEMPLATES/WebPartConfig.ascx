<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebPartConfig.ascx.cs"
    Inherits="Oxbow.GenericDashboard.SharePoint.CONTROLTEMPLATES.WebPartConfig" %>
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
<style>
    .sortableList li
    {
        list-style-type: none;
        list-style-image: none;
        background-color: #ddd;
        padding: 4px;
        border: solid 1px #333;
        margin-left: -40px;
    }
</style>
<script type="text/javascript">
    function ResetOrdering() {
        $(".ulJumpMenuItems li").each(function (i, x) {
            $(x).find("input[id*='hidJumpMenuItemOrderId']").val(i);
        });
        $(".ulTabs li").each(function (i, x) {
            $(x).find("input[id*='hidTabConfigurationOrderId']").val(i);
        });
        $(".ulCustomActions li").each(function (i, x) {
            $(x).find("input[id*='hidRowCustomActionOrderId']").val(i);
        });
    }
    $(document).ready(function () {
        $(".lnkToggleExport").click(function () {
            $(".dvExport").toggle("300");
            return false;
        });
        $(".sortableList").sortable({
            stop: function (event, ui) {
                ResetOrdering();
            }
        });
        $(".addLinkButton").button({ icons: { primary: "ui-icon-plus"} });
        $(".deleteLinkButton").button({ icons: { primary: "ui-icon-trash"} });
        $(".prompt").click(function() { return confirm("Conform delete?"); });
    });
</script>
<fieldset>
    <legend>Jump Menu Items</legend>
    <table class="formTable">
        <tr>
            <td class="formTableLabel">
                Prompt
            </td>
            <td class="formTableValue">
                <asp:TextBox ID="txtJumpMenuPrompt" runat="server" />
            </td>
        </tr>
    </table>
    <asp:LinkButton ID="lnkAddJumpMenuItem" runat="server" CssClass="addLinkButton" Text="New Menu Item"></asp:LinkButton>
    <ul class="ulJumpMenuItems sortableList">
        <asp:Repeater ID="rptJumpMenuItems" runat="server" OnItemCommand="rptJumpMenuItems_ItemCommand">
            <ItemTemplate>
                <li>
                    <asp:LinkButton ID="lnkDeleteJumpMenu" CssClass="deleteLinkButton prompt" runat="server"
                        CommandName="Delete" CommandArgument='<%#Eval("Id") %>' Text="Delete" />
                    <asp:HiddenField ID="hidJumpMenuItemId" runat="server" Value='<%# Eval("Id") %>' />
                    <asp:HiddenField ID="hidJumpMenuItemOrderId" runat="server" Value='<%# Eval("OrderId") %>' />
                    <table class="formTable">
                        <tr>
                            <td class="formTableLabel">
                                Name
                            </td>
                            <td class="formTableValue">
                                <asp:TextBox ID="txtName" runat="server" Text='<%#Eval("Name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="formTableLabel">
                                Url
                            </td>
                            <td class="formTableValue">
                                <asp:TextBox ID="txtUrl" runat="server" Text='<%#Eval("Url") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td class="formTableLabel">
                                Target
                            </td>
                            <td class="formTableValue">
                                <asp:TextBox ID="txtTarget" runat="server" Text='<%#Eval("Target") %>' />
                            </td>
                        </tr>
                    </table>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</fieldset>
<fieldset>
    <legend>Tabs</legend>
    <asp:LinkButton ID="lnkAddTab" CssClass="addLinkButton" runat="server" Text="New Tab"></asp:LinkButton>
    <ul class="ulTabs sortableList">
        <asp:Repeater ID="rptTabs" runat="server" OnItemCommand="rptTabs_ItemCommand">
            <ItemTemplate>
                <li>
                    <asp:HiddenField ID="hidTabConfigurationId" runat="server" Value='<%#Eval("Id") %>' />
                    <asp:HiddenField ID="hidTabConfigurationOrderId" runat="server" Value='<%#Eval("OrderId") %>' />
                    <fieldset>
                        <legend>
                            <%#Eval("Name") %></legend>
                        <asp:LinkButton ID="lnkDeleteTabConfiguration" runat="server" CssClass="deleteLinkButton prompt"
                            CommandName="Delete" CommandArgument='<%#Eval("Id") %>' Text="Delete" />
                        <table class="formTable">
                            <tr>
                                <td class="formTableLabel">
                                    Name
                                </td>
                                <td class="formTableValue">
                                    <asp:TextBox ID="txtName" Text='<%#Eval("Name") %>' runat="server" />
                                </td>
                            </tr>
                        </table>
                        <fieldset>
                            <legend>List/View</legend>
                            <table class="formTable">
                                <tr>
                                    <td class="formTableLabel">
                                        Site Url
                                    </td>
                                    <td class="formTableValue">
                                        <asp:TextBox ID="txtWebUrl" Text='<%#Eval("WebUrl") %>' runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formTableLabel">
                                        List Name
                                    </td>
                                    <td class="formTableValue">
                                        <asp:TextBox ID="txtListName" Text='<%#Eval("ListName") %>' runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formTableLabel">
                                        View Name
                                    </td>
                                    <td class="formTableValue">
                                        <asp:TextBox ID="txtViewName" Text='<%#Eval("ViewName") %>' runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset>
                            <legend>Display Options</legend>
                            <table class="formTable">
                                <tr>
                                    <td class="formTableLabel">
                                        Display Count?
                                    </td>
                                    <td class="formTableValue">
                                        <asp:CheckBox ID="chkDisplayCount" runat="server" Checked='<%#Eval("IsCountDisplayed") %>' />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formTableLabel">
                                        Page Size
                                    </td>
                                    <td class="formTableValue">
                                        <asp:TextBox ID="txtPageSize" runat="server" Text='<%#Eval("PageSize") %>' />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset>
                            <legend>Custom Actions</legend>
                            <asp:LinkButton ID="lnkAddRowCustomAction" CommandName="AddRowCustomAction" CommandArgument='<%#Eval("Id") %>'
                                CssClass="addLinkButton" runat="server" Text="New Custom Action"></asp:LinkButton>
                            <ul class="ulCustomActions sortableList">
                                <asp:Repeater runat="server" ID="rptRowCustomActions" OnItemCommand="rptRowCustomActions_ItemCommand">
                                    <ItemTemplate>
                                        <li>
                                            <asp:LinkButton ID="lnkDeleteRowCustomAction" runat="server" CssClass="deleteLinkButton prompt"
                                                CommandName="Delete" CommandArgument='<%#Eval("Id") %>' Text="Delete" />
                                            <asp:HiddenField ID="hidTabId" runat="server" Value='<%#DataBinder.Eval(Container.Parent.Parent, "DataItem.Id") %>' />
                                            <asp:HiddenField ID="hidRowCustomActionId" runat="server" Value='<%#Eval("Id") %>' />
                                            <asp:HiddenField ID="hidRowCustomActionOrderId" runat="server" Value='<%#Eval("OrderId") %>' />
                                            <table class="formTable">
                                                <tr>
                                                    <td class="formTableLabel">
                                                        Name
                                                    </td>
                                                    <td class="formTableValue">
                                                        <asp:TextBox runat="server" ID="txtRowCustomActionName" Text='<%#Eval("Name") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formTableLabel">
                                                        Assembly
                                                    </td>
                                                    <td class="formTableValue">
                                                        <asp:TextBox runat="server" ID="txtRowCustomAssemblyName" Text='<%#Eval("AssemblyName") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formTableLabel">
                                                        Type
                                                    </td>
                                                    <td class="formTableValue">
                                                        <asp:TextBox runat="server" ID="txtRowCustomTypeName" Text='<%#Eval("TypeName") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </fieldset>
                    </fieldset>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</fieldset>
<a href="#" class="lnkToggleExport">Export/Import</a>
<div class="dvExport" style="display: none">
    <fieldset>
        <legend>Xml Extract</legend>
    </fieldset>
    <fieldset>
        <legend>Import/Export</legend>
        <asp:TextBox TextMode="MultiLine" Rows="4" Width="100%" ID="txtImportExport" runat="server" />
        <asp:Button ID="btnImport" Text="Import" runat="server" />
    </fieldset>
</div>
