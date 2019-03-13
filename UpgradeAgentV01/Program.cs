using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Core.WorkflowEngine;

namespace UpgradeAgentV01
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please supply the web url");
                return;
            }
            var logs = new List<AgentLogItem>();
            using (var site = new SPSite(args[0]))
            {
                using (var web = site.OpenWeb())
                {
                    logs = GweAgentRunner.RunJob(web, (config, list) =>
                                                          {
                                                              new WorkflowConfigurationRepository(list.ParentWeb).Save(list.Title, config);
                                                              return new List<AgentLogItem> {AgentLogItem.Debug(config.Id.ToString())};
                                                          }).ToList();
                }
            }
            foreach (var agentLogItem in logs)
            {
                Console.WriteLine(agentLogItem.Message);
            }
        }
    }
}
