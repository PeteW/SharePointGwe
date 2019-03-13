using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.WorkflowEngine;

namespace Oxbow.Gwe.SharePoint.Features.WebApplicationScopedFeature
{
    [Guid("a435e16b-35da-42b4-9021-e962b6d0c986")]
    public class WebApplicationScopedFeatureEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                var webApplication = (SPWebApplication)properties.Feature.Parent;
                webApplication.WebService.ApplyApplicationContentToLocalServer();
                RemoveJob(webApplication);

                var jobDefinition = new GweTimerJobDefinition(SettingsManager.ApplicationName, webApplication);
                var schedule = new SPMinuteSchedule();
                schedule.BeginSecond = 0;
                schedule.EndSecond = 59;
                schedule.Interval = SettingsManager.DefaultGweTimerJobInterval;
                jobDefinition.Schedule = schedule;
                jobDefinition.IsDisabled = true;
                jobDefinition.Update();
            }
            catch (Exception exp)
            {
                ResolveType.Instance.Of<ILogger>().Error(exp.ToString());
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                var webApplication = (SPWebApplication)properties.Feature.Parent;
                RemoveJob(webApplication);
            }
            catch (Exception exp)
            {
                ResolveType.Instance.Of<ILogger>().Error(exp.ToString());
            }
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties) { }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties) { }

        private void RemoveJob(SPWebApplication webApplication)
        {
            foreach (SPJobDefinition job in webApplication.JobDefinitions)
            {
                if (job.Name == SettingsManager.ApplicationName)
                {
                    job.Delete();
                }
            }
        }
    }
}
