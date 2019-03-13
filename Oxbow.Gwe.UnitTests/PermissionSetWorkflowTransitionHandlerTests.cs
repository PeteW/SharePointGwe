using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class PermissionSetWorkflowTransitionHandlerTests : TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            var action = new PermissionSetWorkflowTransitionAction();
            action.PermissionSet = TestUserLoginDomain + "\\" + TestUserLoginName + @":Administrator";
            action.Execute(listItem);
        }
    }
}