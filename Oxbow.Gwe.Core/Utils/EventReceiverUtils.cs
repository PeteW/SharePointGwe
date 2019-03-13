using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Utils
{
    public class EventReceiverUtils
    {
        public static bool IsEventReceiverRegistered(SPList spList)
        {
            foreach (SPEventReceiverDefinition receiver in spList.EventReceivers)
            {
                if (receiver.Assembly == "Oxbow.Gwe.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0cd9d8500cf32c1c")
                    return true;
            }
            return false;
        }

        public static void Register(SPList spList)
        {
            if (IsEventReceiverRegistered(spList))
                return;
            SPEventReceiverDefinition r = spList.EventReceivers.Add();
            r.Name = "Oxbow.Gwe_ItemAdded";
            r.Assembly = "Oxbow.Gwe.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0cd9d8500cf32c1c";
            r.Class = "Oxbow.Gwe.Core.WorkflowEngine.WorkflowEventReceiver";
            r.Type = SPEventReceiverType.ItemAdded;
            r.SequenceNumber = 10000;
            r.Update();
            SPEventReceiverDefinition r2 = spList.EventReceivers.Add();
            r2.Name = "Oxbow.Gwe_ItemUpdated";
            r2.Assembly = "Oxbow.Gwe.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0cd9d8500cf32c1c";
            r2.Class = "Oxbow.Gwe.Core.WorkflowEngine.WorkflowEventReceiver";
            r2.Type = SPEventReceiverType.ItemUpdated;
            r2.SequenceNumber = 10000;
            r2.Update();
        }
        public static void Unregister(SPList spList)
        {
            if(!IsEventReceiverRegistered(spList))
                return;
            var rec = new List<SPEventReceiverDefinition>();
            foreach (SPEventReceiverDefinition receiver in spList.EventReceivers)
            {
                if (receiver.Assembly == typeof(EventReceiverUtils).Assembly.FullName)
                    rec.Add(receiver);
            }
            foreach (var spEventReceiverDefinition in rec)
            {
                spEventReceiverDefinition.Delete();
            }
        }
    }
}