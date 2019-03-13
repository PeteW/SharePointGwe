using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.Models
{
    public class FormContainer : SpListItemContainer, IFormContainer
    {
        public FormContainer()
        {
        }

        public FormContainer(SPListItem spListItem, WorkflowConfiguration workflowConfiguration) : base(spListItem, workflowConfiguration)
        {
            byte[] formFileBytes = spListItem.File.OpenBinary();
            string xmlString = new UTF8Encoding().GetString(formFileBytes).Trim();
            if (string.IsNullOrEmpty(xmlString))
                throw new Exception("XML was null or empty.");
            ResolveType.Instance.Of<ILogger>().Debug(string.Format("At: [{2}] Deserializing list item #{0}... Xml: {1}", spListItem.ID, xmlString, Environment.StackTrace));
            SpListItem = spListItem;
            ReadXml(xmlString);
        }

        public XmlDocument XmlDocument { get; set; }
        private XmlNamespaceManager XmlNamespaceManager { get; set; }

        #region IFormContainer Members

        public string Xml { get; private set; }

        public string GetValueByXpath(string xPath, string defaultValue)
        {
            XPathNavigator result = GetNodeByXpath(xPath);
            if (result == null)
                return defaultValue;
            return result.Value;
        }

        public void SetNodeToNil(string xPath)
        {
            SetNodeByXpath(xPath, string.Empty);
            GetNodeByXpath(xPath).SetNilAttribute();
        }

        public void SetNodeByXpath(string xPath, string value)
        {
            XPathNavigator node = GetNodeByXpath(xPath);
            if (node == null)
                throw new Exception(string.Format("The node at xpath [{0}] was not found.", xPath));
            node.SetValue(value);
            if (!string.IsNullOrEmpty(value))
                node.RemoveNilAttribute();
//            else
//                node.SetNilAttribute();
        }

        public override void PerformSystemSave()
        {
            if (SpListItem == null)
                throw new Exception("Cannot perform system save if SpListItem is missing.");
            try
            {
                //ensure empty elements are flagged properly.. this seems to be a bug in XmlDocument
                foreach (XmlElement element in XmlDocument.SelectNodes("//*[. = '' and count(*) = 0]"))
                {
                    element.IsEmpty = true;
                }
                using (var fileStream = new MemoryStream())
                {
                    XmlDocument.PreserveWhitespace = true;
                    XmlDocument.Save(fileStream);
                    SpListItem.Web.RunUnsafeWithElevatedPrivileges(w =>
                                                                       {
                                                                           SPList list = w.Lists[SpListItem.ParentList.Title];
                                                                           SPListItem listItem = list.GetItemById(SpListItem.ID);
                                                                           listItem.File.SaveBinary(fileStream);
                                                                       });
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Exception occurred when saving the XML form file back to the list: [{0}]", e));
            }
        }

        public string EvalXpathExpression(string xpathExpression)
        {
            string[] xpNamespaces = CommonCode.GetNamespacesUsedInXpathQuery(xpathExpression);
            foreach (string xpNamespace in xpNamespaces)
            {
                if (string.IsNullOrEmpty(XmlNamespaceManager.LookupNamespace(xpNamespace)))
                    XmlNamespaceManager.AddNamespace(xpNamespace, XmlNamespaceManager.DefaultNamespace);
            }
            return (XmlDocument.CreateNavigator().Evaluate(xpathExpression, XmlNamespaceManager)??string.Empty).ToString();
        }
        
        public XPathNavigator GetNodeByXpath(string xPath)
        {
            string[] xpNamespaces = CommonCode.GetNamespacesUsedInXpathQuery(xPath);
            foreach (string xpNamespace in xpNamespaces)
            {
                if (string.IsNullOrEmpty(XmlNamespaceManager.LookupNamespace(xpNamespace)))
                    XmlNamespaceManager.AddNamespace(xpNamespace, XmlNamespaceManager.DefaultNamespace);
            }
            return XmlDocument.CreateNavigator().SelectSingleNode(xPath, XmlNamespaceManager);
        }

        public void AppendChildXml(string parentNodeXpath, bool prepend, string xml)
        {
            var parentNode = GetNodeByXpath(parentNodeXpath);
            if (parentNode == null)
                throw new Exception(string.Format("Unable to find the parent node using xpath: [{0}]", parentNodeXpath));
            if (prepend)
                parentNode.PrependChild(xml);
            else
                parentNode.AppendChild(xml);
        }

        public void DeleteNode(string xpath)
        {
            var node = GetNodeByXpath(xpath);
            if (node == null)
                throw new Exception(string.Format("Unable to find the node using xpath: [{0}]", xpath));
            node.DeleteSelf();
        }
        

        #endregion

        public static FormContainer CreateFromXmlForTesting(string xml)
        {
            var result = new FormContainer();
            result.ReadXml(xml);
            return result;
        }

        private void ReadXml(string xml)
        {
            Xml = xml;
            //            using (var reader = new XmlTextReader(new StringReader(xml)))
            //            {
            XmlDocument = new XmlDocument();
            XmlDocument.LoadXml(xml);
            //                result.XPathDocument = new XPathDocument(reader);
            XPathNavigator xPathNavigator = XmlDocument.CreateNavigator();
            XmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
            xPathNavigator.MoveToFollowing(XPathNodeType.Element);
            IDictionary<string, string> namespaces = xPathNavigator.GetNamespacesInScope(XmlNamespaceScope.All);
            foreach (var ns in namespaces)
            {
                XmlNamespaceManager.AddNamespace(ns.Key, ns.Value);
            }
            //            }
        }
    }
}