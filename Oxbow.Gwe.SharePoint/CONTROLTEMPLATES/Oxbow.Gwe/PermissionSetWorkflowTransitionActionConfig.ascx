<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.Controls.PermissionSetWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td colspan="2">
            Syntax: [user or group1]:[permissions1],[user or group2]:[permissions2]...
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Example: ${!xpath(my:userName)}:Contributor,AdminGroup:Administrator <a href="http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.sproletype.aspx" target="_blank">Click here for permission types.</a>
        </td>
    </tr>
    <tr>
        <td class="labelCell">
            Permission set
        </td>
        <td>
            <asp:TextBox ID="txtPermissionSet" CssClass="formField" runat="server" />
        </td>
    </tr>
</table>
