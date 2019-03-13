using Oxbow.GenericDashboard.Core.Models;
using NUnit.Framework;

namespace Oxbow.GenericDashboard.UnitTests
{
    [TestFixture]
    public class TestGenericDashboardConfiguration:BaseTest
    {
        [Test]
        public void TestHydrateDehydrate()
        {
            var v1 = new GenericDashboardConfiguration();
            v1.JumpMenuItems.Add(new JumpMenuItem(){Name = "a", OrderId = 1});
            v1.JumpMenuItems.Add(new JumpMenuItem(){Name = "b", OrderId = 2});
            v1.JumpMenuItems.Add(new JumpMenuItem(){Name = "c", OrderId = 3});
            v1.TabConfigurations.Add(new TabConfiguration(){Name = "A", OrderId = 1});
            v1.TabConfigurations.Add(new TabConfiguration(){Name = "B", OrderId = 2});
            v1.TabConfigurations.Add(new TabConfiguration(){Name = "C", OrderId = 3});
            var text = v1.Dehydrate();

            var v2 = GenericDashboardConfiguration.Hydrate(text);
            Assert.That(v1.JumpMenuItems.Count==3);
            Assert.That(v1.JumpMenuItems[0].Name=="a" && v1.JumpMenuItems[0].OrderId==1);
            Assert.That(v1.JumpMenuItems[1].Name=="b" && v1.JumpMenuItems[1].OrderId==2);
            Assert.That(v1.JumpMenuItems[2].Name=="c" && v1.JumpMenuItems[2].OrderId==3);

            Assert.That(v1.TabConfigurations[0].Name == "A" && v1.TabConfigurations[0].OrderId == 1);
            Assert.That(v1.TabConfigurations[1].Name == "B" && v1.TabConfigurations[1].OrderId == 2);
            Assert.That(v1.TabConfigurations[2].Name == "C" && v1.TabConfigurations[2].OrderId == 3);
        }
    }
}