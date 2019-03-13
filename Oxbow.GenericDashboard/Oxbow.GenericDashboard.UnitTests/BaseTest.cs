using System;
using Oxbow.GenericDashboard.Core.Models;
using Microsoft.SharePoint;
using NUnit.Framework;

namespace Oxbow.GenericDashboard.UnitTests
{
    [TestFixture]
    public class BaseTest
    {
        public static readonly string SiteUrl = "http://localhost/GenericDashboard/";
        public static readonly string List1Name = "List1";
        public static readonly string View1Name = "View1";
        public static readonly string View2Name = "View2";
        public GenericDashboardConfiguration CreateTestGenericDashboardConfiguration()
        {
            var g = new GenericDashboardConfiguration();
            g.JumpMenuItems.Add(new JumpMenuItem() { Id = Guid.NewGuid(), Name = "Menu Item1", OrderId = 1, Url = "http://reallifedata.com/" });
            g.JumpMenuItems.Add(new JumpMenuItem() { Id = Guid.NewGuid(), Name = "Menu Item2", OrderId = 2, Url = "http://disney.com/" });
            g.JumpMenuItems.Add(new JumpMenuItem() { Id = Guid.NewGuid(), Name = "Menu Item3", OrderId = 3, Url = "http://oxbowsoftware.com/" });
            g.TabConfigurations.Add(new TabConfiguration() { Id = Guid.NewGuid(), ListName = List1Name, Name = "Tab #1", OrderId = 1, ViewName = View1Name, WebUrl = SiteUrl, PageSize = 1 });
            g.TabConfigurations.Add(new TabConfiguration() { Id = Guid.NewGuid(), ListName = List1Name, Name = "Tab #2", OrderId = 2, ViewName = View2Name, WebUrl = SiteUrl, PageSize = 1 });
            return g;
        }
        #region Setup/Teardown

        [SetUp]
        public void Setup() { Web = new SPSite(SiteUrl).OpenWeb(); }

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

        public SPWeb Web { get; private set; }
    }
}