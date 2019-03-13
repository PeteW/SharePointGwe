using Oxbow.Gwe.Core.Utils;
using NUnit.Framework;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class RegexTests
    {
        [Test]
        public void Test1()
        {
            string testString = "string(/my:personnelRequisitionForm/my:employeeType)";
            string[] namespaces = CommonCode.GetNamespacesUsedInXpathQuery(testString);
            Assert.That(namespaces.Length == 1);
            Assert.That(namespaces[0] == "my");
        }

        [Test]
        public void Test2()
        {
            string testString = "Hello there ${personName} this is your note to ${/my:fields/my:taskDescription}";
            string[] variables = CommonCode.GetCodeBlocksFromTemplate(testString);
            Assert.That(variables.Length == 2);
            Assert.That(variables[0] == "${personName}");
            Assert.That(variables[1] == "${/my:fields/my:taskDescription}");
            Assert.That(CommonCode.GetNamespacesUsedInXpathQuery(variables[1]).Length == 1);
        }
    }
}