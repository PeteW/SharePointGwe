using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Oxbow.Gwe.Lang.Ast;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Utils
{
    public static class SPSecurityExtensions
    {
        public static void RunUnsafeWithElevatedPrivileges(this SPListItem spListItem, SPWeb webContext, Action<SPListItem> listItemAction)
        {
            var listName = spListItem.ParentList.Title;
            webContext.RunUnsafeWithElevatedPrivileges(w =>
            {
                var list = w.Lists[listName];
                var listItem = list.GetItemById(spListItem.ID);
                listItemAction(listItem);
            });
        }
        public static SPList GetSpList(this SPWeb web, string listName)
        {
            try
            {
                web.AssertNotNull("SpWeb");
                return web.Lists[listName];
            }
            catch
            {
                throw new Exception(string.Format("Unable to get a list named [{0}] from the web [{1}]", listName, web.Url));
            }
        }
        public static SPGroup GetSpGroup(this SPWeb web, string groupName)
        {
            web.AssertNotNull("web");
            try
            {
                return web.SiteGroups[groupName];
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("Unable to get the SPGroup named [{0}] from the web [{1}]: {2}", groupName, web.Url, exp));
            }
        }

        public static void RunAs(this SPWeb spWeb, SPUserToken userToken, Action<SPWeb> webAction)
        {
            RunUnsafeWithElevatedPrivileges(spWeb, w =>
            {
                using (var site = new SPSite(w.Url, userToken))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        webAction(web);
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        public static SPUser GetSpUserStrict(this SPListItem li, string fieldName)
        {
            li.AssertNotNull("li");
            try
            {
                var fieldUser = li.Fields[fieldName] as SPFieldUser;
                var fieldValue = fieldUser.GetFieldValue(li[fieldName].ToString()) as SPFieldUserValue;
                if (fieldValue != null)
                    return fieldValue.User;
                throw new Exception("This should not be reached");
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Attempted to extract SPUser from field [{0}] of list item #{1} of list [{2}] when the following exception occurred: {3}", fieldName, li.ID, li.ParentList.Title, exp));
            }
        }
        
        public static SPView GetSpView(this SPList list, string viewName)
        {
            try
            {
                return list.Views[viewName];
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("Unable to read the view [{0}] from the list [{1}]: {2}", viewName, list.Title, exp));
            }
        }

        public static string StripDomainPrecursor(this string input)
        {
            input.AssertNotNull("input");
            if (input.IndexOf("\\") > 0)
            {
                input = input.Substring(input.IndexOf("\\") + 1);
            }
            return input;
        }

        public static SPListItem GetLatestCopy(this SPListItem listItem)
        {
            return listItem.ParentList.GetItemById(listItem.ID);
        }
        public static bool IsGweTrue(this string input)
        {
            if(string.IsNullOrEmpty(input))
                return false;
            input = input.Trim().Trim(new[] {'$', '{', '}'});
            if (input.ToLower() == "true" || input == "1")
                return true;
//            if (input.ToLower() == "false" || input == "0")
                return false;
//            throw new Exception(string.Format("The result [{0}] was expected to fall within the set (true,1,false,0). This is inconclusive and does not reduce to a boolean result.", input));
        }
        public static void AssertNotNull(this object o, string errorMessage)
        {
            if(o==null)
                throw new Exception(errorMessage);
        }

        public static string EvaluateFieldAsString(this SPListItem listItem, string fieldName)
        {
            try
            {
                var o = listItem[fieldName];
                if (o is DateTime)
                    return ((DateTime) o).ToShortDateString();
                if (o == null)
                    return null;
                return o.ToString();
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("Error happened when evaluating field from list item as a string. Field: [{0}] ListItem: [{1}] Exception: {2}", fieldName, listItem.GetDescription(), exp));
            }
        }
        public static string GetDescription(this SPListItem spListItem)
        {
            if (spListItem == null)
                return "NULL";
            return string.Format("Item #{0}: Url [{1}]", spListItem.ID, spListItem.Url);
        }


        public static SPList GetSPListByName(this SPWeb web, string listName)
        {
            try
            {
                return web.Lists[listName];
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Attempted to load the list named [{0}] from the site named [{1}] at [{2}] exception: {3}", listName, web.Name, web.Url, exp));
            }
        }
        public static void RemoveNilAttribute(this XPathNavigator node)
        {
            if (node.MoveToAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance"))
                node.DeleteSelf();
        }
        public static void SetNilAttribute(this XPathNavigator node)
        {
            if (!node.MoveToAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance"))
            {
                node.CreateAttribute("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
            }
        }
        public static string GetAbsoluteUrl(this SPListItem listItem ) { return listItem["EncodedAbsUrl"].ToString(); }
        public static string GetBrowserFormUrl(this SPListItem listItem)
        {
            return string.Format("{0}/_layouts/formserver.aspx?xmlLocation={1}&Source={2}&OpenIn=Browser",
                                 listItem.ParentList.ParentWeb.Url,
                                 listItem.ParentList.ParentWeb.Url + "/" + listItem.File.Url,
                                 listItem.ParentList.ParentWeb.Url
                );
        }
        public static string GetBrowserFormUrl(this SPListItem listItem, string redirectUrl)
        {
            var source = string.Empty;
            if (!string.IsNullOrEmpty(redirectUrl))
                source = "&Source=" + redirectUrl;
            return string.Format("{0}/_layouts/formserver.aspx?xmlLocation={1}{2}&OpenIn=Browser",
                                 listItem.ParentList.ParentWeb.Url,
                                 listItem.ParentList.ParentWeb.Url + "/" + listItem.File.Url,
                                 source
                );
        }
        public static SPContentType GetContentTypeByName(this SPWeb web, string contentTypeName)
        {
            try
            {
                var result = web.AvailableContentTypes[contentTypeName];                
                result.AssertNotNull(string.Format("ContentType [{0}] not found.", contentTypeName));
                return result;
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("web.AvailableContentTypes[{0}] threw the following exception: {1}", contentTypeName, exp));
            }
        }
        public static void AssertFunctionArgumentLength(this Expr.Func func, int argLength)
        {
            if(func.Item2.Length!=argLength)
            {
                throw new Exception(string.Format("The function: [{0}] expects {1} parameters, but in this case it is being called with {2} paramaters: {3}", func.Item1, argLength, func.Item2.Length, func.ToString()));
            }
        }

        public static void RunUnsafeWithElevatedPrivileges(this SPWeb webContext, Action<SPWeb> webAction)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(webContext.Url))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        webAction(web);
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }
    }
}