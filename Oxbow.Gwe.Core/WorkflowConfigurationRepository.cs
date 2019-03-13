using System;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core
{
    public class WorkflowConfigurationRepository
    {
        public WorkflowConfigurationRepository(SPWeb spWeb)
        {
            Web = spWeb;
            string serialized = Web.Properties[SettingsManager.GweWorkflowconfigurationKey];
            if (string.IsNullOrEmpty(serialized))
            {
                WorkflowConfigurationCollection = new WorkflowConfigurationCollection();
            }
            else
            {
                try
                {
                    WorkflowConfigurationCollection = WorkflowConfigurationCollection.DeserializeFromString(serialized);
                }
                catch (Exception exp)
                {
                    string error = string.Format("Error encountered when de-serializing workflow configuration collection. Xml:{0} Error: {1}", serialized, exp);
                    ResolveType.Instance.Of<ILogger>().Error(error);
                    throw new Exception(error);
                }
            }
        }

        public SPWeb Web { get; set; }
        public WorkflowConfigurationCollection WorkflowConfigurationCollection { get; set; }

        public static bool IsSpWebGweEnabled(SPWeb spWeb)
        {
            string serialized = spWeb.Properties[SettingsManager.GweWorkflowconfigurationKey];
            return (!string.IsNullOrEmpty(serialized));
        }

        public WorkflowConfiguration GetWorkflowConfigurationByListName(string listName) { return WorkflowConfigurationCollection.Get(listName); }

        public void Save(string listName, WorkflowConfiguration configuration)
        {
            WorkflowConfigurationCollection.Set(listName, configuration);
            string serialized = WorkflowConfigurationCollection.SerializeToString();
            Web.RunUnsafeWithElevatedPrivileges(w =>
                                                    {
                                                        w.Properties[SettingsManager.GweWorkflowconfigurationKey] = serialized;
                                                        w.Properties.Update();
                                                        w.Update();
                                                    });
        }
    }
}