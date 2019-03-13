using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class LoggingWorkflowTransitionHandlerTests:TestBase
    {
         [Test]
        public void TestBasicFunctionality()
         {
             var action = new ListLogWorkflowTransitionAction();
             var list = Web.Lists[FormLibraryName];
             var listItem = list.Items[0];
             action.ListName = FormHistoryListName;
             action.LogMessage = "Testing 123 ${!xpath(/my:myFields/my:someField)}";
             action.Execute(listItem);
         }
    }
}