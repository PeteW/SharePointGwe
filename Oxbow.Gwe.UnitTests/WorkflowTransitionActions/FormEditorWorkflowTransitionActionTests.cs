using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class FormEditorWorkflowTransitionActionTests : TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var action = new FormEditorWorkflowTransitionAction();
            action.TargetFieldXpath = "/my:myFields/my:someField";
            action.NewValue = "foo";
            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}