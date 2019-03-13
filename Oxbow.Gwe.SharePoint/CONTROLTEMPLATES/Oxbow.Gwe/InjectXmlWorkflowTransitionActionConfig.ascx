<%@ Control Language="C#" AutoEventWireup="true" Inherits="Oxbow.Gwe.SharePoint.Controls.InjectXmlWorkflowTransitionActionConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" %>
<table class="formTable">
    <tr>
        <td class="labelCell">
            Target XPath
        </td>
        <td>
            <asp:TextBox ID="txtTargetXPath" CssClass="formField" runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:RadioButton runat="server" GroupName="rbPrependXml" ID="rbPrependXmlYes" Text="Prepend XML"/>
            <asp:RadioButton runat="server" GroupName="rbPrependXml" ID="rbPrependXmlNo" Text="Append XML"/>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Xml
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox runat="server" TextMode="MultiLine" Rows="6" CssClass="fullWidth" ID="txtXml"/>
        </td>
    </tr>
</table>
