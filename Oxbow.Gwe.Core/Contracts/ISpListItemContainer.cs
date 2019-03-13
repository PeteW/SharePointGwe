using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface ISpListItemContainer
    {
        void PerformSystemSave();
        SPListItem SpListItem { get; set; }
        SPListItem SpListItemStrict { get;  }
        WorkflowConfiguration WorkflowConfigurationStrict { get; }
    }
}