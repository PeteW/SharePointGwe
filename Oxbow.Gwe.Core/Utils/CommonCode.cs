using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Utils
{
    public class CommonCode
    {
        public static string GetStringFromResource(Assembly assembly,string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }            
        }
        
        public static string[] GetNamespacesUsedInXpathQuery(string xpathQuery)
        {
            var result = new List<string>();
            var regex = new Regex("/([^:]*):");
            var matches = regex.Matches(xpathQuery);
            foreach (Match match in matches)
            {
                foreach (Group g in match.Groups)
                {
                    var x = g.Value;
                    result.Add(x.Replace(":","").Replace("/",""));
                }
            }
            return result.Distinct().ToArray();
        }
        
//        public static string[] GetVariablesFromTemplate(string template) { return GetVariables(template).Distinct().Select(x => x.Replace("${", "").Replace("}", "")).ToArray(); }
        public static string[] GetEquationsFromTemplate(string template)
        {
            template.AssertNotNull("Template was null");
            return GetCodeBlocksFromTemplate(template).Select(x => x.Trim(new[] {'$', '{', '}'})).ToArray();
        }
        public static string[] GetCodeBlocksFromTemplate(string template)
        {
            template.AssertNotNull("Template was null");
            template = template.Trim();
            var result = new List<string>();
            var regex = new Regex(@"\${([^}]*)}");
            var matches = regex.Matches(template);
            foreach (Match match in matches)
            {
                result.Add(match.Groups[0].Value);
            }
            return result.ToArray();
        }

        public string ReplaceVariableWithValue(string template, string variable, string value) { return template.Replace(variable, value); }

        public static string GetStringFromResource(string resourceName) { return GetStringFromResource(typeof (CommonCode).Assembly, resourceName); }

        /// <summary>
        /// Sets the permissions on list item.
        /// </summary>
        /// <param name="spListItem">The sp list item.</param>
        /// <param name="principalsAndRoleTypes">The principals and role types.</param>
        public static void SetPermissionsOnListItem(SPListItem spListItem, IDictionary<SPPrincipal, SPRoleType> principalsAndRoleTypes)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var site = new SPSite(spListItem.Web.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;

                            SPList list = web.Lists[spListItem.ParentList.Title];
                            SPListItem listItem = list.GetItemById(spListItem.ID);

                            //break any of the standard inheritance
                            if (listItem.HasUniqueRoleAssignments == false)
                            {
                                listItem.BreakRoleInheritance(true);
                                web.AllowUnsafeUpdates = true; //calling BreakRoleInheritance(true) will automatically reset the AllowUnsafeUpdates. not sure if that is a bug or not, but here I fix it.
                            }
                            //blow away any pre-existing permissions, start from scratch
                            while (listItem.RoleAssignments.Count > 0)
                            {
                                listItem.RoleAssignments.Remove(0);
                            }
                            foreach (var principalAndRoleType in principalsAndRoleTypes)
                            {
                                //get a role definition (admin/contributor/read)
                                SPRoleDefinition roleDefinition = listItem.ParentList.ParentWeb.RoleDefinitions.GetByType(principalAndRoleType.Value);
                                //create a role assignment for the user (the principal)
                                var roleAssignment = new SPRoleAssignment(principalAndRoleType.Key);
                                //add the role definition to the new role assignment
                                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                //add the role assignments to the list item
                                listItem.RoleAssignments.Add(roleAssignment);
                            }
                            listItem.SystemUpdate(false);
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error occurred while setting the list item permissions: [{0}]", ex));
            }
        }

    }
}