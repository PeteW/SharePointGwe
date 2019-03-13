using System;
using System.Collections.Generic;
using System.Text;
using Oxbow.GenericDashboard.Core.Contracts;
using Oxbow.GenericDashboard.Core.Utils;
using Microsoft.SharePoint;

namespace Oxbow.GenericDashboard.Core
{
    public abstract class BaseSpFieldRenderer: ISpFieldRenderer
    {
        public string RenderHtml(SPListItem spListItem, SPField spField, bool first)
        {
            var val = RenderHtml(spListItem, spField);
            if(first)
                val = WrapListItemUrl(string.Format("<img src='{0}' border='0' /> {1}", spListItem.GetIconUrl(), val), spListItem);
            return val;
        }
        

        public abstract string RenderExcel(SPListItem spListItem, SPField spField);
        public abstract string RenderHtml(SPListItem spListItem, SPField spField);
        public virtual string GetListItemUrl(SPListItem spListItem)
        {
            if(spListItem.ParentList.BaseTemplate==SPListTemplateType.XMLForm && spListItem.ParentList.DefaultItemOpen==DefaultItemOpen.Browser)
                return spListItem.GetBrowserFormUrl();
            if (spListItem.IsFile() || spListItem.IsFolder())
                try
                {
                    return spListItem.Web.Url + "/" + spListItem.Url;//spListItem[SPBuiltInFieldId.EncodedAbsUrl].ToString();
                }
                catch
                {
                    spListItem.GetDispFormUrl();
                }
            return spListItem.GetDispFormUrl();
        }
        string WrapListItemUrl(string input, SPListItem spListItem) { return string.Format("<a href='{0}'>{1}</a>", GetListItemUrl(spListItem), input); }
    }
    public class DateTimeSpFieldRenderer:BaseSpFieldRenderer
    {
        private string RenderInternal(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            try
            {
                return DateTime.Parse(input).ToShortDateString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public override string RenderExcel(SPListItem spListItem, SPField spField) { return RenderInternal(spListItem.GetFieldAsString(spField)); }

        public override string RenderHtml(SPListItem spListItem, SPField spField) { return RenderInternal(spListItem.GetFieldAsString(spField)); }
    }
    
    public class UserSpFieldRenderer:BaseSpFieldRenderer
    {
        private string RenderInternal(SPListItem spListItem, SPField spField)
        {
            try
            {
                var currentValue = spListItem[spField.Title];
                if (currentValue == null)
                    return "N/A";
                try
                {
                    var l = new List<string>();
                    var users = ((SPFieldUserValueCollection)currentValue);
                    foreach (SPFieldUserValue user in users)
                    {
                        if(user.User==null || user.User.Name==null)
                            l.Add(user.LookupValue);
                        else
                            l.Add(user.User.Name);
                    }
                    return string.Join(", ", l.ToArray());
                }
                catch
                {
                }
                var singleUserField = spField as SPFieldUser;
                var fieldValue = (SPFieldUserValue)spField.GetFieldValue(currentValue.ToString());
                return fieldValue.User.Name;
            }
            catch
            {
                return "N/A";
            }
        }
        public override string RenderExcel(SPListItem spListItem, SPField spField){return RenderInternal(spListItem, spField);}

        public override string RenderHtml(SPListItem spListItem, SPField spField) { return RenderInternal(spListItem, spField); }
    }
    public class ChoiceSpFieldRenderer:BaseSpFieldRenderer
    {
        public override string RenderExcel(SPListItem spListItem, SPField spField) { return spListItem.GetFieldAsString(spField).TrimStart(new char[]{';','#'}).Replace(";#",", ").TrimEnd(new char[]{',', ' '}); }

        public override string RenderHtml(SPListItem spListItem, SPField spField) { return spListItem.GetFieldAsString(spField).TrimStart(new char[] { ';', '#' }).Replace(";#", ", ").TrimEnd(new char[] { ',',' ' }); }
    }
    public class DummySpFieldRenderer:BaseSpFieldRenderer
    {
        public override string RenderExcel(SPListItem spListItem, SPField spField) { return spListItem.GetFieldAsFormattedString(spField); }

        public override string RenderHtml(SPListItem spListItem, SPField spField) { return spListItem.GetFieldAsFormattedString(spField); }
    }
}