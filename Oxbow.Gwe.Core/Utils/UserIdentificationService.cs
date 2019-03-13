using System;
using System.Collections.Generic;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.Core.Utils
{
    public class UserIdentificationService : IUserIdentificationService
    {
        #region IUserIdentificationService Members

        public SPUser GetSPUserByEmail(string email, SPWeb webContext)
        {
            SPPrincipalInfo spPrincipalInfo = null;

            webContext.RunUnsafeWithElevatedPrivileges((x) => { spPrincipalInfo = SPUtility.ResolvePrincipal(x.Site.WebApplication, null, email, SPPrincipalType.User, SPPrincipalSource.All, true); });
            if (spPrincipalInfo == null)
                throw new Exception(string.Format("Unable to locate/ensure a sharepoint identity for the given email: [{0}]. ", email));

            SPUser spUser = null;
            webContext.RunUnsafeWithElevatedPrivileges((x) => { spUser = x.EnsureUser(spPrincipalInfo.LoginName); });
            return spUser;
        }

        public SPUser GetSPUserByUserName(string userName, SPWeb webContext)
        {
            try
            {
                SPUser spUser = null;
                webContext.RunUnsafeWithElevatedPrivileges(w => spUser = w.EnsureUser(userName));
                return spUser;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to locate/ensure a sharepoint identity for the given name: [{0}] message: [{1}]", userName, e));
            }
        }

        public SPGroup GetSPGroupByName(string groupName, SPWeb webContext)
        {
            try
            {
                return webContext.SiteGroups[groupName];
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to locate/ensure a sharepoint group with the given name: [{0}] message: [{1}]", groupName, e));
            }
        }

        public SPUser GetManager(string userName, SPWeb webContext)
        {
            try
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(webContext.Site);
                var userProfileManager = new UserProfileManager(serviceContext);
                SPUser user = GetSPUserByUserName(userName, webContext);
                UserProfile userProfile = userProfileManager.GetUserProfile(userName);
                return GetSPUserByUserName(userProfile.GetManager()["AccountName"].ToString(), webContext);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to find the manager for user [{0}]: {1}", userName, e));
            }
        }

        //Modified by : CKMahesh Kumar
        //Modified Date: 07-Feb-2013
        //Purpose : To set access restrictions for DALVendors. used Runwithelevated...
        public bool IsUserMemberOfSpGroup(string userName, string groupName, SPWeb webContext)
        {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    webContext.AllowUnsafeUpdates = true;
                    SPGroup secGroup = webContext.SiteGroups["DALVendors"];
                    SPPrincipal principal = (SPPrincipal)secGroup;
                    SPRoleAssignment roleAss = webContext.RoleAssignments.GetAssignmentByPrincipal(principal);
                    roleAss.RoleDefinitionBindings.RemoveAll();
                    roleAss.Update();
                    SPRoleAssignment roleAssignment = new SPRoleAssignment(secGroup);
                    SPRoleDefinition roleDefinition;
                    roleDefinition = webContext.RoleDefinitions["Read"];
                    roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                    webContext.RoleAssignments.Add(roleAssignment);
                    secGroup.Update();
                    webContext.AllowUnsafeUpdates = false;
                });
            try
            {
                    Boolean IsUserInGroup = false;
                    var spUser = webContext.EnsureUser(userName);
                    using (var site = new SPSite(webContext.Url, spUser.UserToken))
                    {
                        using (var web1 = site.OpenWeb())
                        {
                            var spGroup = web1.GetSpGroup(groupName);
                            IsUserInGroup = web1.IsCurrentUserMemberOfGroup(spGroup.ID);
                        }
                    }

                    webContext.AllowUnsafeUpdates = true;
                    SPGroup secGroup = webContext.SiteGroups["DALVendors"];
                    SPPrincipal principal = (SPPrincipal)secGroup;
                    SPRoleAssignment roleAss = webContext.RoleAssignments.GetAssignmentByPrincipal(principal);
                    roleAss.RoleDefinitionBindings.RemoveAll();
                    roleAss.Update();
                    webContext.AllowUnsafeUpdates = false;

                    return IsUserInGroup;
            }
            catch (Exception e)
            {
                var err = string.Format("Unable to check if user [{0}] is a member of group [{1}]. The following exception occurred: {2}", userName, groupName, e);
                ResolveType.Instance.Of<ILogger>().Error(err);
                return false;
            }
        }

        #endregion
    }
}