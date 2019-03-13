using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Administration;
using NUnit.Framework;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Core.WorkflowEngine;
//using Oxbow.Gwe.Core.WorkflowEngine;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class TimerJobTests : TestBase
    {
        private void PrintLogs(List<AgentLogItem> logItems)
        {
            foreach (AgentLogItem timerJobLogItem in logItems)
            {
                Console.WriteLine("{0} - {1} : {2}", timerJobLogItem.Time, timerJobLogItem.Severity, timerJobLogItem.Message);
            }
        }

        [Test]
        public void RunWebApp()
        {
            var logs = new List<AgentLogItem>();
            Web.RunUnsafeWithElevatedPrivileges(w =>
                                                    {
                                                        SPWebApplication webApp = Web.Site.WebApplication;
                                                        logs = GweAgentRunner.RunJob(webApp, TimeTriggerJobRunner.RunTimerJob).ToList();
                                                    });
            PrintLogs(logs.ToList());
        }
    }
}