using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Oxbow.GenericDashboard.Core.Models;

namespace Oxbow.GenericDashboard.UnitTests
{
    [TestFixture]
    public class CustomActionHandlerTest
    {
        [Test]
        public void Test()
        {
            var customAction = new RowCustomAction();
            customAction.AssemblyName = "Oxbow.GenericDashboard.Core, Culture=Neutral, Version=1.0.0.0, PublicKeyToken=8168d5868e89fd50";
            customAction.TypeName = "Oxbow.GenericDashboard.Core.Utils.DummyRowCustomActionHandler";
            customAction.Execute(null);
        }

    }
}