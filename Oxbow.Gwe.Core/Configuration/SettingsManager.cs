using System;
using System.Configuration;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Configuration
{
    public class SettingsManager
    {
        public static bool IsDebugMode { get { return false; } }
        public static bool IsEmailEnabled { get { return true; } }
        public static string ApplicationName { get { return "Oxbow.Gwe"; } }
        public static readonly string GweWorkflowconfigurationKey = "Gwe.WorkflowConfigurations";
        public static readonly int DefaultGweTimerJobInterval = 1;


        public static bool IsRunningInSharePoint
        {
            get
            {
                try
                {
                    return SPContext.Current.Web != null;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static string TestSiteUrl{get { return GetSetting("TestSiteUrl"); }}

        public static bool IsUnitTestMode
        {
            get
            {
                try
                {
                    return Boolean.Parse(GetSetting("IsUnitTestMode"));
                }
                catch
                {
                    return false;
                }
            }
        }


        private static string GetSetting(string setting)
        {
            string test = ConfigurationManager.AppSettings[setting];
            if (test == null)
                throw new Exception(String.Format("Unable to find an appsetting defined for [{0}]. Maybe the path to the appSettings file is incorrect, or maybe the specific setting is missing within the Appsettings file.", setting));
            return test;
        }
    }
}