using Microsoft.SharePoint;

namespace Oxbow.GenericDashboard.Core.Contracts
{
    public interface ISpFieldRenderer
    {
        string RenderHtml(SPListItem spListItem, SPField spField, bool first);
        string RenderExcel(SPListItem spListItem, SPField spField);
    }
}