using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Office.InfoPath.Server.Controls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.SharePoint.Layouts.Oxbow.Gwe
{
    class XmlManipulationKit
    {
        public XPathNavigator XPathNavigator { get; set; }
        public XmlNamespaceManager XmlNamespaceManager { get; set; }
    }

    public partial class GweFormViewer : WebPartPage
    {
        private string XsnLocation
        {
            get { return Request["Xsn"]; }
        }

        private string XmlLocation
        {
            get { return Request["Xml"]; }
        }

        public string Redirect
        {
            get { return Request["Redirect"]; }
        }

        private XmlManipulationKit GetXmlManipulationToolsFromXmlFormViewer()
        {
            var xPathNavigator = XmlFormView1.XmlForm.MainDataSource.CreateNavigator();
            var xNameSpace = new XmlNamespaceManager(new NameTable());
            xNameSpace.AddNamespace("my", XmlFormView1.XmlForm.NamespaceManager.LookupNamespace("my"));
            var result = new XmlManipulationKit();
            result.XPathNavigator = xPathNavigator;
            result.XmlNamespaceManager = xNameSpace;
            return result;
        }

        protected override void OnLoad(EventArgs e)
        {
            XmlFormView1.Close += new EventHandler(XmlFormView1_Close);
            XmlFormView1.Initialize += new EventHandler<InitializeEventArgs>(XmlFormView1_Initialize);
            XmlFormView1.SubmitToHost += new EventHandler<SubmitToHostEventArgs>(XmlFormView1_SubmitToHost);
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(XmlLocation))
                    XmlFormView1.XmlLocation = XmlLocation;//"/OrderDesktopLaptop/2012-10-11T11_18_47pweissbrod.xml
                else if (!string.IsNullOrEmpty(XsnLocation))
                    XmlFormView1.XsnLocation = XsnLocation;//"/OrderDesktopLaptop/Forms/template.xsn";
                else
                    throw new Exception("Either the Xsn or Xml parameters must be supplied in the URL");
            }
            base.OnLoad(e);
        }

        void XmlFormView1_SubmitToHost(object sender, SubmitToHostEventArgs e)
        {
            var kit = GetXmlManipulationToolsFromXmlFormViewer();
            var commandNode = kit.XPathNavigator.SelectSingleNode("/my:myFields/my:SubmitCommand", kit.XmlNamespaceManager);
            if(commandNode!=null && commandNode.Value.ToLower()=="refresh")
            {
                var q = new QueryString(Request.RawUrl);
                var xmlPath = XmlFormView1.XmlForm.Uri;
                if(!Uri.IsWellFormedUriString(xmlPath, UriKind.RelativeOrAbsolute))
                {
                    var fileNameNode = kit.XPathNavigator.SelectSingleNode("/my:myFields/my:FormName", kit.XmlNamespaceManager);
                    var formLibraryUrlNode = kit.XPathNavigator.SelectSingleNode("/my:myFields/my:FormLibraryUrl", kit.XmlNamespaceManager);
                    var fileName = fileNameNode.Value + ".xml";
                    xmlPath = SPContext.Current.Web.Url + formLibraryUrlNode.Value + fileName;
                }
                q["Xsn"] = "";
                q["Xml"] = xmlPath;
                Response.Redirect(q.AllUrl);

            }
        }


        void XmlFormView1_Initialize(object sender, InitializeEventArgs e)
        {
            var parameters = Request["Parameters"];
            if (!string.IsNullOrEmpty(parameters))
            {
                try
                {
                    var kit = GetXmlManipulationToolsFromXmlFormViewer();
                    kit.XPathNavigator.SelectSingleNode("/my:myFields/my:QueryStringParameters", kit.XmlNamespaceManager).SetValue(parameters);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("The following error occurred when pulling data from the request [{0}] into the form: {1}", parameters, exp));
                }
            }
        }

        void XmlFormView1_Close(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Redirect))
                Response.Redirect(Redirect);
//            Response.Redirect("/SitePages/MyShoppingCart.aspx");
        }
    }
}
