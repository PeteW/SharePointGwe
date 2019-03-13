<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.Controls.FormEditorWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td class="labelCell">
            Values
        </td>
        <td>
            <div class="buttonHeader">
                <asp:Button runat="server" ID="btnAddItem" Text="+" />
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table class="fullWidth">
                <tr>
                    <td>
                        Target Field Xpath
                    </td>
                    <td>
                        New Value
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rptItems">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:HiddenField runat="server" ID="hidId" Value='<%#Eval("Id") %>' />
                                <asp:TextBox runat="server" ID="txtTargetXPath" CssClass="formField" Text='<%#Eval("TargetFieldXpath") %>' />
                            </td>
                            <td>
                                <asp:TextBox ID="txtNewValue" CssClass="formField" runat="server" Text='<%#Eval("NewValue") %>' />
                                <asp:Button runat="server" ID="btnDeleteItem" CommandName="Delete" CommandArgument='<%#Eval("Id") %>'
                                    Text="X" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </td>
    </tr>
</table>
