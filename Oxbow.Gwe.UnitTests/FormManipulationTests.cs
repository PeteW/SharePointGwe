using System.IO;
using System.Xml;
using System.Xml.XPath;
using Oxbow.Gwe.Core.Utils;
using NUnit.Framework;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class FormManipulationTests
    {
        [Test]
        public void TestFormManipulationPart1()
        {
            var xml = CommonCode.GetStringFromResource(typeof (FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm1.xml");
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                var xPathDocument = new XPathDocument(reader);
                var xPathNavigator = xPathDocument.CreateNavigator();
                var namespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
                xPathNavigator.MoveToFollowing(XPathNodeType.Element);
                var namespaces = xPathNavigator.GetNamespacesInScope(XmlNamespaceScope.All);
                foreach (var ns in namespaces)
                {
                    namespaceManager.AddNamespace(ns.Key, ns.Value);
                }
                var xpath = "/my:personnelRequisitionForm/my:employeeType";
                var xpNamespaces = CommonCode.GetNamespacesUsedInXpathQuery(xpath);
                foreach (var xpNamespace in xpNamespaces)
                {
                    if (string.IsNullOrEmpty(namespaceManager.LookupNamespace(xpNamespace)))
                        namespaceManager.AddNamespace(xpNamespace, namespaceManager.DefaultNamespace);
                }
                var node = xPathDocument.CreateNavigator().SelectSingleNode(xpath, namespaceManager).Value;
                Assert.That(node == "1", node);
            }
        }
    }
}