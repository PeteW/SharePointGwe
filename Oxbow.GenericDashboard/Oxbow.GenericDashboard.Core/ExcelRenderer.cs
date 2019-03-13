using System.Linq;
using Oxbow.GenericDashboard.Core.Configuration;
using Oxbow.GenericDashboard.Core.Models;
using Oxbow.GenericDashboard.Core.Utils;
using Microsoft.SharePoint;
using SmartXLS;

namespace Oxbow.GenericDashboard.Core
{
    public class ExcelRenderer
    {
        public WorkBook Render(GenericDashboardConfiguration genericDashboardConfiguration)
        {
            var w = new WorkBook();
            w.NumSheets = genericDashboardConfiguration.TabConfigurations.Count;
            var sheet = 0;
            foreach (var tabConfiguration in genericDashboardConfiguration.TabConfigurations.OrderBy(x => x.OrderId))
            {
                SharePointExtensions.Run(tabConfiguration.WebUrl, x =>
                {
                    var list = x.GetSPListByName(tabConfiguration.ListName);
                    var view = list.GetSPViewByName(tabConfiguration.ViewName);
                    var listItems = list.GetItems(new SPQuery(view));
                    var count = listItems.Count;
                    var sheetName = tabConfiguration.Name;
                    if (tabConfiguration.IsCountDisplayed)
                        sheetName += string.Format(" ({0})", count);
                    w.setSheetName(sheet, sheetName);
                    if (listItems.Count == 0)
                        return;
                    //header
                    for (var col = 0; col < view.ViewFields.Count; col++)
                    {
                        var internalFieldName = view.ViewFields[col];
                        var spField = listItems[0].Fields.GetFieldByInternalName(internalFieldName);
                        w.setText(sheet, 0, col, spField.Title);
                    }
                    //rows
                    for (var row = 0; row < listItems.Count; row++)
                    {
                        var spListItem = listItems[row];
                        for (var col = 0; col < view.ViewFields.Count; col++)
                        {
                            var internalFieldName = view.ViewFields[col];
                            var spField = spListItem.Fields.GetFieldByInternalName(internalFieldName);
                            var renderer = ResolveType.Instance.OfSpFieldRenderer(spField.Type);
                            var value = renderer.RenderExcel(spListItem, spField);
                            w.setText(sheet, row + 1, col, value);
                        }
                    }
                });
                sheet++;
            }
            return w;
        }
    }
}