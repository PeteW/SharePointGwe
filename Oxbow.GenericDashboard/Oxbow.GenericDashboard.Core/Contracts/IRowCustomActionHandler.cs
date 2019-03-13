using Microsoft.SharePoint;

namespace Oxbow.GenericDashboard.Core.Contracts
{
    public interface IRowCustomActionHandler
    {
        void Execute(SPListItem listItem, string Action); 
    }
}