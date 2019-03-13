using Oxbow.Gwe.Core.Models;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface IWorkflowTransitionHandler
    {
        void RunTransition(SPWeb webContext, WorkflowTransition workflowTransition, FormContainer formContainer);
    }
}