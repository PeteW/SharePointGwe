using System.Diagnostics;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Utils
{
    public class EventLogLogger : ILogger
    {
        public SettingsManager SettingsManager { get; private set; }

        #region ILogger Members

        public void Error(string errorMessage)
        {
            Log(errorMessage, EventLogEntryType.Error);
        }

        public void Debug(string message)
        {
            if (!SettingsManager.IsDebugMode)
                return;
            Log("Debug: " + message, EventLogEntryType.Information);
        }

        public void Info(string errorMessage)
        {
            Log(errorMessage, EventLogEntryType.Information);
        }

        #endregion

        private void Log(string message, EventLogEntryType eventLogEntryType)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                if (EventLog.SourceExists(SettingsManager.ApplicationName) == false)
                    EventLog.CreateEventSource(SettingsManager.ApplicationName, "Application");
                EventLog.WriteEntry(SettingsManager.ApplicationName, message, eventLogEntryType);
            });
        }
    }
}