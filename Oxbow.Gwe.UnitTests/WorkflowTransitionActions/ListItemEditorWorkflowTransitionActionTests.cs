using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class ListItemEditorWorkflowTransitionActionTests : TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var action = new ListItemEditorWorkflowTransitionAction();
            action.TargetFieldExpression = "Title";
            action.NewValueExpression = "foo";
            SPList list = Web.Lists[FormLibraryName];
            SPListItem listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}