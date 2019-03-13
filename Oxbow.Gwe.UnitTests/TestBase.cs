using Microsoft.SharePoint;
using NSubstitute;
using NUnit.Framework;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class TestBase
    {
        protected ISpListItemContainer MockSpListItemContainer{get { return Substitute.For<ISpListItemContainer>(); }}
        protected ISpListItemContainer MockFormContainer{get { return Substitute.For<IFormContainer>(); }}
        protected ISpListItemContainer GetFormContainer()
        {
            var list = Web.Lists[FormLibraryName];
            var listItem = list.Items[0];
            var formContainer = ResolveType.Instance.OfSpListItemContainer(listItem,null);
            return formContainer;
        }

        #region Setup/Teardown

        [SetUp]
        public void Setup() { Web = new SPSite(WebUrl).OpenWeb(); }

        [TearDown]
        public void TearDown()
        {
            if (Web != null)
            {
                Web.Dispose();
                Web = null;
            }
        }

        #endregion
        /// <summary>
        /// To set all the tests up:
        /// 1. make a sub site called gwe
        /// 2. make a form library in there (make sure you enable GWE within the form library to properly test the agent runner)
        /// 3. deploy the form template in the /TestFormTemplates
        /// 4. make a custom list to hold the form history
        /// 5. configure the variables below to match the names of your site and lists
        /// </summary>
        public SPWeb Web { get; private set; }
        public readonly string WebUrl = "http://localhost/gwe/";
        public readonly string FormLibraryName = "FormLibrary";
        public readonly string CustomListName = "TheDocSet";
        public readonly string TestSharePointGroupName = "TestGroup";
        public readonly string FormHistoryListName = "Form History";
        public readonly string TestUserLoginName = "jondoe";
        public readonly string TestUserLoginDomain = "spdev";
        public readonly string TestUserEmail = "spadmin@spdev.local";
        public readonly string FromEmailAddress = "reallifedata@gmail.com";
        public readonly string ToEmailAddress = "reallifedata@gmail.com";
    }
}