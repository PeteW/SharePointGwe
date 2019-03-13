using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowEngine
{
    public class GweAgentRunner
    {
        /// <summary>
        /// run the job on the web application
        /// </summary>
        /// <param name="webApplication"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public static IList<AgentLogItem> RunJob(SPWebApplication webApplication, Func<WorkflowConfiguration, SPList, IList<AgentLogItem>> job)
        {
            var logs = new List<AgentLogItem>();
            logs.Add(AgentLogItem.Debug(string.Format("Running timer job on site collection [{0}]", webApplication.Name)));
            foreach (SPSite site in webApplication.Sites)
            {
                logs.AddRange(RunJob(site, job));
                site.Close();
            }
            return logs;
        }
        /// <summary>
        /// run the job on the site
        /// </summary>
        /// <param name="site"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public static IList<AgentLogItem> RunJob(SPSite site, Func<WorkflowConfiguration, SPList, IList<AgentLogItem>> job)
        {
            var logs = new List<AgentLogItem>();
            logs.Add(AgentLogItem.Debug(string.Format("Running timer job on site collection [{0}]", site.Url)));
            foreach (SPWeb web in site.AllWebs)
            {
                logs.AddRange(RunJob(web, job));
                web.Close();
            }
            return logs;
        }
        /// <summary>
        /// run the job on the web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public static IList<AgentLogItem> RunJob(SPWeb web, Func<WorkflowConfiguration, SPList, IList<AgentLogItem>> job)
        {
            var logs = new List<AgentLogItem>();
            logs.Add(AgentLogItem.Debug(string.Format("Running timer job on web [{0}]", web.Url)));
            if (!WorkflowConfigurationRepository.IsSpWebGweEnabled(web))
            {
                logs.Add(AgentLogItem.Debug(string.Format("GWE is not configured for the web [{0}].", web.Url)));
                return logs;
            }
            foreach (SPList list in web.Lists)
            {
                logs.AddRange(RunJob(list, job));
            }
            return logs;
        }
        /// <summary>
        /// run the job on the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public static IList<AgentLogItem> RunJob(SPList list, Func<WorkflowConfiguration, SPList, IList<AgentLogItem>> job)
        {
            var logs = new List<AgentLogItem>();
            //            logs.Add(AgentLogItem.Debug(string.Format("Running timer job on web [{0}], list [{1}]", list.Title, web.Url)));
            if (WorkflowConfigurationRepository.IsSpWebGweEnabled(list.ParentWeb))
            {
                WorkflowConfigurationRepository repository;
                try
                {
                    repository = new WorkflowConfigurationRepository(list.ParentWeb);
                }
                catch (Exception e)
                {
                    logs.Add(AgentLogItem.Error(string.Format("Exception encountered when opening GWE config on: {0}: {1}", list.ParentWebUrl, e)));
                    return logs;
                }
                WorkflowConfiguration workflowConfiguration = repository.GetWorkflowConfigurationByListName(list.Title);
                if (workflowConfiguration == null)
                {
                    logs.Add(AgentLogItem.Debug(string.Format("No workflow configuration found for the list titled [{0}]", list.Title)));
                    return logs;
                }
                logs.Add(AgentLogItem.Debug(string.Format("Executing job on list [{0}] web [{1}]", list.Title, list.ParentWebUrl)));
                var start = DateTime.Now;
                logs.AddRange(job(workflowConfiguration, list));
                logs.Add(AgentLogItem.Debug(string.Format("Job on list [{0}] web [{1}] completed in {2} seconds.", list.Title, list.ParentWebUrl, DateTime.Now.Subtract(start).TotalSeconds)));
            }
            else
            {
                logs.Add(AgentLogItem.Debug(string.Format("The web [{0}] is not gwe enabled so it will be skipped.", list.ParentWebUrl)));
            }
            return logs;
        }

    }
}