<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.CONTROLTEMPLATES.Oxbow.Gwe.ListItemEditorWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td class="labelCell">
            Target Field
        </td>
        <td>
            <asp:TextBox ID="txtTargetFieldExpression" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            New Value
        </td>
        <td>
            <asp:TextBox ID="txtNewValueExpression" CssClass="formField" runat="server" />
        </td>
    </tr>
</table>
