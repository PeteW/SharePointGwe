using System;
using System.Web;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.Core.Utils
{
    public class UserIdentity
    {
        public static string GetCurrentUserName()
        {
            try
            {
                return HttpContext.Current.User.Identity.Name;
            }
            catch
            {
                var localusername = Environment.UserName;
                if (!localusername.Contains("\\"))
                {
                    localusername = Environment.UserDomainName + "\\" + Environment.UserName;
                }
                return localusername;
            }
        }
        public static string GetUserNameFromUserIdentityString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            //claims-based identity encoding: http://www.wictorwilen.se/Post/How-Claims-encoding-works-in-SharePoint-2010.aspx
            //example: i:0#.w|ninjaspecial\pweissbrod -> ninjaspecial\pweissbrod
            if (input.IndexOf("|") != -1)
                input = input.Substring(input.IndexOf("|") + 1);
            //windows-based identity encoding
            //example: ninjaspecial\pweissbrod
            if (input.IndexOf("\\") != -1)
                return input.Split(new char[] { '\\' })[1];
            //unable to find the domain name, default to current domain name
            throw new Exception(string.Format("The input [{0}] was not recognized as claims format nor windows.", input));            
        }
        
        public static string GetDomainNameFromUserIdentityString(string input, string defaultValue)
        {
            try
            {
                return GetDomainNameFromUserIdentityString(input);
            }
            catch(Exception exp)
            {
                ResolveType.Instance.Of<ILogger>().Error(string.Format("Exception occurred when trying to get the domain name from the user identity string [{0}]. The default value of ["+defaultValue+"] will be returned as a substitute. Exception: {1}. ", input, exp));
                return defaultValue;
            }
        }

        public static string GetUserNameFromUserIdentityString(string input, string defaultValue)
        {
            try
            {
                return GetUserNameFromUserIdentityString(input);
            }
            catch(Exception exp)
            {
                ResolveType.Instance.Of<ILogger>().Error(string.Format("Exception occurred when trying to get the user name from the user identity string [{0}]. The default value of ["+defaultValue+"] will be returned as a substitute. Exception: {1}. ", input, exp));
                return defaultValue;
            }
        }

        public static string GetDomainNameFromUserIdentityString(string input)
        {
            if(string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");
            //claims-based identity encoding: http://www.wictorwilen.se/Post/How-Claims-encoding-works-in-SharePoint-2010.aspx
            //example: i:0#.w|ninjaspecial\pweissbrod -> ninjaspecial\pweissbrod
            if (input.IndexOf("|") != -1)
                input = input.Substring(input.IndexOf("|") + 1);
            //windows-based identity encoding
            //example: ninjaspecial\pweissbrod
            if (input.IndexOf("\\") != -1)
                return input.Split(new char[] {'\\'})[0];
            //unable to find the domain name, default to current domain name
            throw new Exception(string.Format("The input [{0}] was not recognized as claims format nor windows.", input));
        }
    }
}
