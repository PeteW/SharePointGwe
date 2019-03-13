using Microsoft.SharePoint;
using NUnit.Framework;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class UserIdentificationServiceTests:TestBase
    {
        [Test]
        public void TestGetSPUserByEmail()
        {
            var userIdentificationService = ResolveType.Instance.Of<IUserIdentificationService>();
            var user = userIdentificationService.GetSPUserByEmail(TestUserEmail,Web);
            Assert.IsNotNull(user);
        }
        [Test]
        public void TestGetSPUserByUserName()
        {
            var userIdentificationService = ResolveType.Instance.Of<IUserIdentificationService>();
            var user = userIdentificationService.GetSPUserByUserName(TestUserLoginName,Web);
            Assert.IsNotNull(user);
        }
        [Test]
        public void TestGetSPGroupByName()
        {
            var userIdentificationService = ResolveType.Instance.Of<IUserIdentificationService>();
            var group = userIdentificationService.GetSPGroupByName(TestSharePointGroupName,Web);
            Assert.IsNotNull(group);
        }
        [Test]
        public void TestGetManager()
        {
            var userIdentificationService = ResolveType.Instance.Of<IUserIdentificationService>();
            var manager = userIdentificationService.GetManager(TestUserLoginName, Web);
            Assert.IsNotNull(manager);

        }
        [Test]
        public void TestIsUserMemberOfSharePointGroup()
        {
            var userIdentificationService = ResolveType.Instance.Of<IUserIdentificationService>();
            var result = userIdentificationService.IsUserMemberOfSpGroup(TestUserLoginName, TestSharePointGroupName, Web);
            Assert.IsTrue(result,string.Format("Make sure that [{0}] is a member of [{1}] on [{2}]", TestUserLoginName, TestSharePointGroupName, Web.Url));
        }
    }
}