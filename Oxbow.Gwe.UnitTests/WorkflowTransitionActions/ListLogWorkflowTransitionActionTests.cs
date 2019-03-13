using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class ListLogWorkflowTransitionActionTests : TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var action = new ListLogWorkflowTransitionAction();
            action.ListName = "Gwe History";
            action.LogMessage = "Unit test";

            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}