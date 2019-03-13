using NUnit.Framework;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class FormHistoryItemTests : TestBase
    {
        [Test]
        public void TestGetFormHistory()
        {
            FormHistoryItem[] result = FormHistoryItem.GetFormHistory(Web, FormHistoryListName, "Form.xml");
        }
    }
}