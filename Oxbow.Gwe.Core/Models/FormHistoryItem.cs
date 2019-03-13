using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Models
{
    [DataContract]
    public class FormHistoryItem
    {
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string Description { get; set; }

        public static FormHistoryItem[] GetFormHistory(SPWeb webContext, string historyListName, string formTitle)
        {
            SPList list = null;
            try
            {
                list = webContext.Lists[historyListName];
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("The following error occured when trying to load the history list titled [{0}] : {1}", historyListName, exp));
            }
            SPListItemCollection spListItemCollection = list.GetItems(new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>{0}</Value></Eq></Where><OrderBy><FieldRef Name='Created' Ascending='False'/></OrderBy>", formTitle) });
            var result = new List<FormHistoryItem>();
            foreach (SPListItem spListItem in spListItemCollection)
            {
                var item = new FormHistoryItem();
                item.Date = (DateTime)spListItem[SPBuiltInFieldId.Created];
                item.Description = spListItem["Description"].ToString();
                result.Add(item);
            }
            return result.ToArray();
        }
    }
}