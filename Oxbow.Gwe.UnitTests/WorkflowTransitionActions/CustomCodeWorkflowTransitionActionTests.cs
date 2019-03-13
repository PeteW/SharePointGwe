using NUnit.Framework;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.UnitTests.WorkflowTransitionActions
{
    [TestFixture]
    public class CustomCodeWorkflowTransitionActionTests:TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            //assumption: the Oxbow.Gwe.TestCustomWorkflowTransitionAction.dll is in the GAC
            var action = new CustomCodeWorkflowTransitionAction();
            action.TypeName = "Oxbow.Gwe.TestCustomWorkflowTransitionAction.Test";
            action.AssemblyName = "Oxbow.Gwe.TestCustomWorkflowTransitionAction, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9d73ad88c24f8009";

            var list = Web.Lists[FormLibraryName];
            var listItem = list.Items[0];
            action.Execute(listItem);
        }
    }
}