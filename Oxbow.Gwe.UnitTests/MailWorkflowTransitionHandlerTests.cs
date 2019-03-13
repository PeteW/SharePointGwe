using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowEngine;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class MailWorkflowTransitionHandlerTests:TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var list = Web.Lists[FormLibraryName];
            var listItem = list.Items[0];
            var action = new EmailWorkflowTransitionAction();
            action.Body = "Testing 123 ${!xpath(/my:myFields/my:someField)}";
            action.From = FromEmailAddress;
            action.Subject = "Email action test";
            action.TargetRecipients = ToEmailAddress;
            action.Execute(listItem);
        }
    }
}