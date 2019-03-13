using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oxbow.Gwe.Core;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.WorkflowEngine;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class GweAgentRunnerTests:TestBase
    {
        [Test]
        public void TestBasicFunctionality()
        {
            var agentLogItems = GweAgentRunner.RunJob(Web, (w, l) =>
                                           {
                                               return new List<AgentLogItem> {AgentLogItem.Debug(w.Id.ToString())};
                                           });
            foreach (var agentLogItem in agentLogItems)
            {
                Console.WriteLine(agentLogItem.Message);
            }
        }
    }
}