using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class EmailWorkflowTransitionActionTests : TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var action = new EmailWorkflowTransitionAction();
            action.Body = "Hello from gwe";
            action.From = "test@wsslab.local";
            action.Subject = "Test of EmailWorkflowTransitionAction";
            action.TargetRecipients = "pweissbrod@oxbowsoftware.com";
            action.TargetCc = "talktopete@gmail.com";

            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}