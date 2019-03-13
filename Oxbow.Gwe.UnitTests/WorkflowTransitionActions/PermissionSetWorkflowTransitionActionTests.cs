using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class PermissionSetWorkflowTransitionActionTests:TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var action = new PermissionSetWorkflowTransitionAction();
            action.PermissionSet = "GroupX:Contributor";

            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}