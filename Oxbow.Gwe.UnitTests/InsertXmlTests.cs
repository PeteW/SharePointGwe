using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class InsertXmlTests : TestBase
    {
        public static void InsertXml(XPathNavigator xpn, string xmlFormat, bool insertBefore, params string[] vals)
        {
        }

        [Test]
        public void Run()
        {
            var appendXml = "<my:hello />";
            var xml = CommonCode.GetStringFromResource(typeof(FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm1.xml");
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                var xPathDocument = new XmlDocument();
                xPathDocument.Load(reader);
                var xPathNavigator = xPathDocument.CreateNavigator();
                var namespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
                xPathNavigator.MoveToFollowing(XPathNodeType.Element);
                var namespaces = xPathNavigator.GetNamespacesInScope(XmlNamespaceScope.All);
                foreach (var ns in namespaces)
                {
                    namespaceManager.AddNamespace(ns.Key, ns.Value);
                }
                var xpath = "/my:personnelRequisitionForm/my:hrSection";
                var xpNamespaces = CommonCode.GetNamespacesUsedInXpathQuery(xpath);
                foreach (var xpNamespace in xpNamespaces)
                {
                    if (string.IsNullOrEmpty(namespaceManager.LookupNamespace(xpNamespace)))
                        namespaceManager.AddNamespace(xpNamespace, namespaceManager.DefaultNamespace);
                }
                var pathNavigator = xPathDocument.CreateNavigator().SelectSingleNode(xpath, namespaceManager);
                Assert.IsNotNull(pathNavigator);
                pathNavigator.PrependChild(appendXml);
                Console.WriteLine(pathNavigator.OuterXml);
            }
        }
    }
}