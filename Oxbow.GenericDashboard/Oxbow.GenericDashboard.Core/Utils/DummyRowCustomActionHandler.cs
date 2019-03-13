using Microsoft.SharePoint;
using Oxbow.GenericDashboard.Core.Configuration;
using Oxbow.GenericDashboard.Core.Contracts;

namespace Oxbow.GenericDashboard.Core.Utils
{
    public class DummyRowCustomActionHandler:IRowCustomActionHandler
    {
        public void Execute(SPListItem listItem,string Action)
        {
        }
    }
}