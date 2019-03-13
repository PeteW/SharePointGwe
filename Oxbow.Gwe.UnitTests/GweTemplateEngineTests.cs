using System;
using NSubstitute;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.TemplateEngine;
using Oxbow.Gwe.Core.Utils;
using NUnit.Framework;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class GweTemplateEngineTests:TestBase
    {
        [Test]
        public void TestVariableReplacement()
        {
            var xml = CommonCode.GetStringFromResource(typeof (FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm2.xml");
            var formContainer = FormContainer.CreateFromXmlForTesting(xml);
            var template = "Herro mr. ${jones}! you are looking at Requisition #${abc}";
            var result = ResolveType.Instance.Of<ITemplateEngine>().Render(template, formContainer);
            Assert.That(result == "Herro mr. jones! you are looking at Requisition #abc");
        }
        [Test]
        public void TestVariableReplacement2()
        {
            var xml = CommonCode.GetStringFromResource(typeof (FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm2.xml");
            var formContainer = FormContainer.CreateFromXmlForTesting(xml);
            var template = "Herro mr. ${!xpath(/my:personnelRequisitionForm/my:reportsTo)}! you are looking at Requisition #${!xpath(/my:personnelRequisitionForm/my:hrSection/my:requisitionNumber)}";
            var result = ResolveType.Instance.Of<ITemplateEngine>().Render(template, formContainer);
            Assert.That(result == "Herro mr. Assign to Director! you are looking at Requisition #2");
        }
        [Test]
        public void TestGetClientFormUrl()
        {
            var formContainer = GetFormContainer();
            var url = ResolveType.Instance.Of<ITemplateEngine>().Render("${!getclientformurl()}", formContainer);
            Assert.NotNull(url);
            Console.WriteLine(url);
        }
        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestGetClientFormUrl2()
        {
            var formContainer = GetFormContainer();
            var url = ResolveType.Instance.Of<ITemplateEngine>().Render("${!getclientformurl(foo)}", formContainer);
            Assert.NotNull(url);
            Console.WriteLine(url);
        }
        [Test]
        public void TestGetBrowserFormUrl()
        {
            var formContainer = GetFormContainer();
            var url = ResolveType.Instance.Of<ITemplateEngine>().Render("${!getbrowserformurl()}", formContainer);
            Assert.NotNull(url);
            Console.WriteLine(url);
        }
        [Test]
        public void ResolveUserEmail()
        {
            var formContainer = GetFormContainer();
            var email = ResolveType.Instance.Of<ITemplateEngine>().Render(string.Format("${{!resolveUserEmail({0}, {1})}}", TestUserLoginName, TestUserLoginDomain), formContainer);
            Assert.NotNull(email);
            Console.WriteLine(email);
        }
        [Test]
        [ExpectedException(typeof(Exception))]
        public void ResolveUserEmail2()
        {
            var formContainer = GetFormContainer();
            var email = ResolveType.Instance.Of<ITemplateEngine>().Render(string.Format("${{!resolveUserEmail({0}, foobar)}}", TestUserLoginName), formContainer);
            Assert.NotNull(email);
            Console.WriteLine(email);
        }
        [Test]
        public void TestIf()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!if(true,a,b)}", MockSpListItemContainer), "a");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!if(false,a,b)}", MockSpListItemContainer), "b");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!if(\"\",a,b)}", MockSpListItemContainer), "b");
        }
        [Test]
        public void TestAnd()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!and(true,true,false)}", MockSpListItemContainer).ToLower(),"false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!and(true)}", MockSpListItemContainer).ToLower(),"true");
        }
        [Test]
        public void TestOr()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!or(true,false,false)}", MockSpListItemContainer).ToLower(),"true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!or(true)}", MockSpListItemContainer).ToLower(),"true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!or(false)}", MockSpListItemContainer).ToLower(),"false");
        }
        [Test]
        public void TestIsContentType()
        {
            var formContainer = GetFormContainer();
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!iscontenttype(Form)}", formContainer).ToLower(), "true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!iscontenttype(Document)}", formContainer).ToLower(), "true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!iscontenttype(Event)}", formContainer).ToLower(), "false");
        }
        [Test]
        public void TestListItemField()
        {
            var formContainer = GetFormContainer();
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!listitemfield(Name)}", formContainer).ToLower(), "form.xml","Expected spListItem[Name] = 'form.xml', actual was: ["+formContainer.SpListItem["Name"]+"]");
        }
        [Test]
        public void TestEquals()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!equals(1,1)}",MockFormContainer).ToLower(),"true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!equals(1,0)}",MockFormContainer).ToLower(),"false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!equals(true,!and(true,true))}",MockFormContainer).ToLower(),"true");
        }
        [Test]
        public void TestConcatenate()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!concatenate(abcde,ab)}", MockFormContainer).ToLower(), "abcdeab");
        }
        [Test]
        public void TestContains()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!contains(abcde,ab)}",MockFormContainer).ToLower(),"true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!contains(abcde,ce)}",MockFormContainer).ToLower(),"false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!contains(!concatenate(ab,true,cde),true)}",MockFormContainer).ToLower(),"true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!if(!or(!contains( \";#United States\", \"United States\"),!contains( \";#United States\", \"Canada\")), ITAM, GCS)}", MockFormContainer).ToLower(), "itam");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!if(!or(!contains( \";#India\", \"United States\"),!contains( \";#India\", \"Canada\")), ITAM, GCS)}", MockFormContainer).ToLower(), "gcs");
        }
        [Test]
        public void TestGetOperandsFromFunctionCall()
        {
            var operands = ResolveType.Instance.Of<ITemplateEngine>().GetOperandsFromFunctionCall(" ${!listitemfieldzz(xxx,yyy)} ", MockSpListItemContainer);
            Assert.AreEqual(operands.Count,2);
            Assert.That(operands.Contains("xxx"));
            Assert.That(operands.Contains("yyy"));
        }
        [Test]
        public void TestAdd()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!add(1,1)}", MockSpListItemContainer).Trim(), "2");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!add(1.1,1.1)}", MockSpListItemContainer).Trim(), "2.2");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!add(1.1,1)}", MockSpListItemContainer).Trim(), "2.1");
        }
        [Test]
        public void TestAddDate()
        {
            //!addDate ( !xpath(\my:fields\my:date), 1, days)
            var today = DateTime.Today;
            Console.WriteLine(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,days)} ", MockFormContainer));
            Console.WriteLine(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,hours)} ", MockFormContainer));
            Console.WriteLine(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,minutes)} ", MockFormContainer));
            Console.WriteLine(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,seconds)} ", MockFormContainer));
            Console.WriteLine(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",-365,days)} ", MockFormContainer));
            
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",-365,days)} ", MockFormContainer).Trim(), today.AddDays(-365).ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,days)} ", MockFormContainer).Trim(), today.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,hours)} ", MockFormContainer).Trim(), today.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,minutes)} ", MockFormContainer).Trim(), today.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss"));
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render(" ${!addDate(\"" + today + "\",1,seconds)} ", MockFormContainer).Trim(), today.AddSeconds(1).ToString("yyyy-MM-ddTHH:mm:ss"));
        }
        [Test]
        public void TestNot()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!not(!or(true,false,false))}", MockSpListItemContainer).ToLower(), "false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!not(true)}", MockSpListItemContainer).ToLower(), "false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!not(false)}", MockSpListItemContainer).ToLower(), "true");
        }
        [Test]
        public void TestNotEquals()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!notequals(1,1)}", MockFormContainer).ToLower(), "false");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!notequals(1,0)}", MockFormContainer).ToLower(), "true");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!notequals(true,!and(true,true))}", MockFormContainer).ToLower(), "false");
        }
        [Test]
        public void TestHtmlEncode()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!htmlencode(<html><head><title>t</title></head></html>)}", MockFormContainer).Trim(), "&lt;html&gt;&lt;head&gt;&lt;title&gt;t&lt;/title&gt;&lt;/head&gt;&lt;/html&gt;");
        }
        [Test]
        public void TestHtmlDecode()
        {
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!htmldecode(&lt;html&gt;&lt;head&gt;&lt;title&gt;t&lt;/title&gt;&lt;/head&gt;&lt;/html&gt;)}", MockFormContainer).Trim(), "<html><head><title>t</title></head></html>");
        }
        [Test]
        public void TestEvalXpath()
        {
            var xml = CommonCode.GetStringFromResource(typeof(FormManipulationTests).Assembly, "Oxbow.Gwe.UnitTests.Resources.ExampleForm2.xml");
            var formContainer = FormContainer.CreateFromXmlForTesting(xml);
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!evalxpath(\"count(/my:personnelRequisitionForm/my:approverSection/my:approver[my:isApproved='true'])\")}", formContainer).ToLower(), "1");
            Assert.AreEqual(ResolveType.Instance.Of<ITemplateEngine>().Render("${!evalxpath(\"count(/my:personnelRequisitionForm/my:approverSection/my:approver[my:isApproved='true'])* 200\")}", formContainer).ToLower(), "200");
        }
    }
}