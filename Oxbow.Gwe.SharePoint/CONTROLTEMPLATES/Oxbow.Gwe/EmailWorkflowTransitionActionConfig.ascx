<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.Controls.EmailWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td class="labelCell">
            Target Recipients
        </td>
        <td>
            <asp:TextBox ID="txtTargetRecipients" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            Target CC
        </td>
        <td>
            <asp:TextBox ID="txtTargetCc" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            From
        </td>
        <td>
            <asp:TextBox ID="txtFrom" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            Subject
        </td>
        <td>
            <asp:TextBox ID="txtSubject" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            Body
        </td>
        <td>
            <asp:TextBox ID="txtBody" CssClass="formField htmlEditor" TextMode="MultiLine" Rows="12" runat="server" />
        </td>
    </tr>
</table>
