using System;
using Oxbow.GenericDashboard.Core;
using Oxbow.GenericDashboard.Core.Models;
using Oxbow.GenericDashboard.Core.Utils;
using Microsoft.SharePoint;
using NUnit.Framework;

namespace Oxbow.GenericDashboard.UnitTests
{
    [TestFixture]
    public class DashboardRendererTests:BaseTest
    {
        [Test]
        public void TestRender1()
        {
            var d = new DashboardRenderer();
            var list = Web.GetSPListByName(List1Name);
            var view = list.GetSPViewByName(View1Name);
            var query = new SPQuery(view);
            var items = list.GetItems(query);
            var testGenericDashboardConfiguration = CreateTestGenericDashboardConfiguration();
            var result = d.Render(testGenericDashboardConfiguration, Web);
            Console.WriteLine(result.JumpMenuHtml);
            foreach (var table in result.DashboardTabs)
            {
                Console.WriteLine(table.TabCaption);
                Console.WriteLine(table.DataTablesDefinition);
                Console.WriteLine(table.TableHtml);
            }
        }
    }
}