using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface IWorkflowTransitionCustomAction
    {
        void Execute(SPListItem listItem);
    }
}