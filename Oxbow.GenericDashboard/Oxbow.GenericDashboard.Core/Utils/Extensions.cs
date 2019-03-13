using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Microsoft.SharePoint;

namespace Oxbow.GenericDashboard.Core.Utils
{
    public static class SharePointExtensions
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

        public static SPListItem GetLatestCopy(this SPListItem listItem)
        {
            return listItem.ParentList.GetItemById(listItem.ID);
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
        public static SPView GetSPViewByName(this SPList list, string viewName)
        {
            try
            {
                return list.Views[viewName];
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Attempted to load the view named [{0}] from the list named [{1}] at [{2}] exception: {3}", viewName, list.Title, list.ParentWebUrl, exp));
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
        public static string GetAbsoluteUrl(this SPListItem listItem) { return listItem["EncodedAbsUrl"].ToString(); }
        public static string GetBrowserFormUrl(this SPListItem listItem) // updated the source by Neeti dated 6th March 2012, so that once provisioners complete the task, they will get redirected to provisioners page rather than home page
        {
            return string.Format("{0}/_layouts/formserver.aspx?xmlLocation={1}&Source={2}&OpenIn=Browser",
                                 listItem.ParentList.ParentWeb.Url,
                                 listItem.ParentList.ParentWeb.Url + "/" + listItem.File.Url,
                                 listItem.ParentList.ParentWeb.Url + System.Web.HttpContext.Current.Request.Path
                );
        }
        public static bool IsFolder(this SPListItem listItem) { return listItem.Folder != null; }
        public static bool IsFile(this SPListItem listItem) { return listItem.File != null; }

        public static T GetValue<T>(this SPListItem listItem, string columnName, T defaultValue)
        {
            try
            {
                return GetValue<T>(listItem, columnName);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T GetValue<T>(this SPListItem listItem, string columnName)
        {
            try
            {
                return (T)listItem[listItem.ParentList.Fields.GetField(columnName).Id];
            }
            catch (InvalidCastException e)
            {
                throw new Exception(string.Format("An exception occurred when trying to read from a list item from list: [{0}] with ID: [{1}] field name: [{2}] expected type: [{3}] ACTUAL type: [{4}] more information: [{5}]",
                                                  listItem.ParentList.Title,
                                                  listItem.ID,
                                                  columnName,
                                                  typeof(T),
                                                  listItem[listItem.ParentList.Fields.GetField(columnName).Id].GetType(),
                                                  e
                                                  ));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("An exception occurred when trying to read from a list item from list: [{0}] with ID: [{1}] field name: [{2}] expected type: [{3}] more information: [{4}]",
                                                  listItem.ParentList.Title,
                                                  listItem.ID,
                                                  columnName,
                                                  typeof(T),
                                                  e
                                                  ));
            }



        }

        public static string GetIconUrl(this SPListItem listItem)
        {
            if (listItem.IsFolder())
                return @"\_layouts\Images\Folder.gif";
            var icon = listItem.GetValue<string>("DocIcon", null);
            return icon == null ? @"\_layouts\Images\ICgen.gif" : string.Format(@"\_layouts\Images\IC{0}.gif", listItem["DocIcon"]);
        }

        public static string GetFieldAsString(this SPListItem listItem, SPField spField)
        {
            try
            {
                return listItem[spField.Id].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetFieldAsFormattedString(this SPListItem listItem, SPField spField)
        {
            try
            {
                return listItem.GetFormattedValue(spField.Title);
            }
            catch
            {
                    return listItem.GetFieldAsString(spField);
            }
        }
        public static string GetDispFormUrl(this SPListItem spListItem)
        {
            return string.Format("{0}/{1}?id={2}", spListItem.Web.Url, spListItem.ParentList.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url, spListItem.ID);
        }

        public static string GetUrl(this SPListItem spListItem)
        {
            try
            {
                return new SPFieldUrlValue(spListItem[SPBuiltInFieldId.EncodedAbsUrl].ToString()).Url;
            }
            catch
            {
                return spListItem.Url;
            }
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
        
        public static SPListItem GetSpListItemById(this SPList list, int id)
        {
            list.AssertNotNull("SPList");
            try
            {
                return list.GetItemById(id);
            }
            catch
            {
                throw new Exception(string.Format("Unable to find list item #{0} within the list [{1}]", id, list.Title));
            }
        }

        public static void AssertNotNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

        public static T SingleOrError<T>(this IEnumerable<T> @this, Func<T, bool> predicate, string errorPredicate)
        {
            var result = @this.Where(predicate).ToList();
            if(result.Count>1)
                throw new Exception(errorPredicate+ "(more than one match found)");
            if(result.Count<1)
                throw new Exception(errorPredicate+ "(no matches found)");
            return result.First();
        }
        public static void RunUnsafeWithElevatedPrivileges(string url, Action<SPWeb> webAction)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(url))
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

        public static void Run(string url, Action<SPWeb> webAction)
        {
                using (var site = new SPSite(url))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        webAction(web);
                        web.AllowUnsafeUpdates = false;
                    }
                }
        }

        public static void RunUnsafeWithElevatedPrivileges(this SPWeb webContext, Action<SPWeb> webAction)
        {
            RunUnsafeWithElevatedPrivileges(webContext.Url, webAction);
        }
    }
}