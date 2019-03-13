using System;
using NUnit.Framework;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class FormContainerTests : TestBase
    {
        [Test]
        public void TestDeleteNode()
        {
            var xml = CommonCode.GetStringFromResource(typeof(FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm2.xml");
            var formContainer = FormContainer.CreateFromXmlForTesting(xml);
            formContainer.DeleteNode("/my:personnelRequisitionForm/my:approverSection/my:approver[my:isApproved='true']");
            Console.WriteLine(formContainer.GetNodeByXpath("/my:personnelRequisitionForm/my:approverSection").OuterXml);
        }
        [Test]
        public void TestAppendXml()
        {
            var xml = CommonCode.GetStringFromResource(typeof(FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm2.xml");
            var formContainer = FormContainer.CreateFromXmlForTesting(xml);
            var xmli = "<approver><approverDisplayName>Weissbrod, Peter</approverDisplayName><approverEmail>pweissbrod@xxx.com</approverEmail><approverUserName>XXX\\pweissbrod</approverUserName><isApproved>true</isApproved><approverComments /><notesFromHr /></approver>"; 
            formContainer.AppendChildXml("/my:personnelRequisitionForm/my:approverSection",true, xmli);
            Console.WriteLine(formContainer.GetNodeByXpath("/my:personnelRequisitionForm/my:approverSection").OuterXml);

        }
    }
}
