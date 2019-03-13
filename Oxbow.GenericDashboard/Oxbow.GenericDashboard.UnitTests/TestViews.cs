using System;
using Oxbow.GenericDashboard.Core.Utils;
using Microsoft.SharePoint;
using NUnit.Framework;

namespace Oxbow.GenericDashboard.UnitTests
{
    [TestFixture]
    public class TestViews:BaseTest
    {
        [Test]
        public void Run()
        {
            var list = Web.GetSPListByName(List1Name);
            var view = list.GetSPViewByName(View1Name);
            Console.WriteLine(view.Query);
            var query = new SPQuery(view);
            var items = list.GetItems(query);
        }
    }
}