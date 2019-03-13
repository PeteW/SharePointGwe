<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.Controls.ListLogWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td class="labelCell">
            List name
        </td>
        <td>
            <asp:TextBox ID="txtListName" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            Log message
        </td>
        <td>
            <asp:TextBox ID="txtLogMessage" CssClass="formField" runat="server" />
        </td>
    </tr>
</table>
