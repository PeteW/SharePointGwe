<%@ Assembly Name="Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c" %>
<%@ Import Namespace="Oxbow.Gwe.Core.Models" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GweFormViewer.aspx.cs" Inherits="Oxbow.Gwe.SharePoint.Layouts.Oxbow.Gwe.GweFormViewer, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5" MasterPageFile="~/_layouts/v4.master" %>
<%@ Register TagPrefix="cc1" Namespace="Microsoft.Office.InfoPath.Server.Controls" Assembly="Microsoft.Office.InfoPath.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
  <style type="text/css">
      .formViewer {
          overflow-y: hidden;
          overflow-x: hidden;
      }
  </style>
  <script type="text/javascript">
      $(document).ready(function () {
          setTimeout(function () {
              var height = $(".formViewer").find("div[id$=XmlFormView1__XmlFormView]").first().height() + 50;
              $(".formViewer").css("height", height + "px");
          }, 3000);
      });
  </script>
  <div id="contentWrap">  
    <cc1:XmlFormView ID="XmlFormView1" CssClass="formViewer" runat="server" Height="1200" Width="835"/>
  </div>

</asp:Content>


