using System;
using System.Runtime.Serialization;
using Oxbow.Gwe.Core.Configuration;

namespace Oxbow.Gwe.Core.Models
{
    [DataContract]
    public class AgentLogItem
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public AgentLogItemSeverity Severity { get; set; }

        public static AgentLogItem Debug(string message) { return new AgentLogItem() {Message = message, Severity = AgentLogItemSeverity.Debug, Time = DateTime.Now}; }
        public static AgentLogItem Info(string message) { return new AgentLogItem() {Message = message, Severity = AgentLogItemSeverity.Info, Time = DateTime.Now}; }
        public static AgentLogItem Warn(string message) { return new AgentLogItem() {Message = message, Severity = AgentLogItemSeverity.Warn, Time = DateTime.Now}; }
        public static AgentLogItem Error(string message) { return new AgentLogItem() {Message = message, Severity = AgentLogItemSeverity.Error, Time = DateTime.Now}; }
    }
}