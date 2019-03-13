using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.TestCustomWorkflowTransitionAction
{
    public class Test : IWorkflowTransitionCustomAction
    {
        #region IWorkflowTransitionCustomAction Members

        public void Execute(SPListItem listItem)
        {
            ResolveType.Instance.Of<ILogger>().Info("Custom action has successfully received an item " + listItem.Url);
        }

        #endregion
    }
}