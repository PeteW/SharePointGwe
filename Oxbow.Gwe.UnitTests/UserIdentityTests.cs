using NUnit.Framework;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class UserIdentityTests:TestBase
    {
         [Test]
        public void TestGetDomainNameFromUserIdentityString()
         {
             Assert.AreEqual("domain", UserIdentity.GetDomainNameFromUserIdentityString("domain\\user"));
             Assert.AreEqual("corp", UserIdentity.GetDomainNameFromUserIdentityString("i:0#.w|corp\\accwalid"));
             Assert.AreEqual("corp", UserIdentity.GetDomainNameFromUserIdentityString("accwalid","corp"));
         }
         [Test]
        public void TestGetUserNameFromUserIdentityString()
         {
             Assert.AreEqual("user", UserIdentity.GetUserNameFromUserIdentityString("domain\\user"));
             Assert.AreEqual("accwalid", UserIdentity.GetUserNameFromUserIdentityString("i:0#.w|corp\\accwalid"));
             Assert.AreEqual("corp", UserIdentity.GetUserNameFromUserIdentityString("accwalid", "corp"));
         }
    }
}