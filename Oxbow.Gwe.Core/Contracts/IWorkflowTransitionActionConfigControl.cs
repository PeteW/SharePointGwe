using System;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface IWorkflowTransitionActionConfigControl<T>
    {
        void UpdateUi(T action);
        void UpdateDataModel(T action);
    }
    public interface IWorkflowTransitionActionConfigControlCallback
    {
        EventHandler<GweActionConfigUserControlCallbackEventArgs> Callback { get; set; } 
    }
    public class GweActionConfigUserControlCallbackEventArgs:EventArgs
    {
        public string CommandName { get; set; }
        public object CommandArgs { get; set; }
    }
}