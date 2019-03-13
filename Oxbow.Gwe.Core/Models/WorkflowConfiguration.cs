using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Oxbow.Gwe.Core.WorkflowTransitionActions;

namespace Oxbow.Gwe.Core.Models
{
    [DataContract, Serializable]
    public class WorkflowConfigurationCollection
    {
        private static XmlSerializer _serializer;

        public WorkflowConfigurationCollection()
        {
            Id = Guid.NewGuid();
            WorkflowConfigurationElements = new WorkflowConfigurationElement[0];
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public WorkflowConfigurationElement[] WorkflowConfigurationElements { get; set; }

        private static XmlSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new XmlSerializer(typeof (WorkflowConfigurationCollection));
                }
                return _serializer;
            }
        }

        public WorkflowConfiguration Get(string listName)
        {
            foreach (WorkflowConfigurationElement k in WorkflowConfigurationElements)
            {
                if (k.ListName == listName)
                    return k.WorkflowConfiguration;
            }
            return null;
        }

        public void Set(string listName, WorkflowConfiguration workflowConfiguration)
        {
            List<WorkflowConfigurationElement> collection = WorkflowConfigurationElements.ToList();
            collection.RemoveAll(x => x.ListName == listName);
            collection.Add(new WorkflowConfigurationElement {ListName = listName, WorkflowConfiguration = workflowConfiguration});
            WorkflowConfigurationElements = collection.ToArray();
        }

        public string SerializeToString()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new XmlTextWriter(stream, Encoding.UTF8);
                Serializer.Serialize(writer, this);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static WorkflowConfigurationCollection DeserializeFromString(string serialized)
        {
            using (var reader = new XmlTextReader(new StringReader(serialized)))
            {
                return (WorkflowConfigurationCollection) Serializer.Deserialize(reader);
            }
        }
    }

    [DataContract, Serializable]
    public class WorkflowConfigurationElement
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string ListName { get; set; }

        public WorkflowConfigurationElement() { Id = Guid.NewGuid(); }

        [DataMember]
        public WorkflowConfiguration WorkflowConfiguration { get; set; }
    }

    [DataContract, Serializable]
    public class WorkflowConfiguration
    {
        private static XmlSerializer _serializer;
        public WorkflowConfiguration()
        {
            Id = Guid.NewGuid();
            WorkflowTransitions = new WorkflowTransition[0];
            WorkflowTimeTriggers = new WorkflowTimeTrigger[0];
            WorkflowConfigurationVariables= new WorkflowConfigurationVariable[0];
        }

        [DataMember]
        public string AdminToEmail { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string AdminFromEmail { get; set; }

        [DataMember]
        public string AdminCcEmail { get; set; }

        [DataMember]
        public string SelectedActionExpression { get; set; }

        [DataMember]
        public WorkflowTransition[] WorkflowTransitions { get; set; }

        [DataMember]
        public WorkflowTimeTrigger[] WorkflowTimeTriggers { get; set; }

        [DataMember]
        public WorkflowConfigurationVariable[] WorkflowConfigurationVariables { get; set; }

        private static XmlSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new XmlSerializer(typeof (WorkflowConfiguration));
                }
                return _serializer;
            }
        }

        public void AddWorkflowTimeTrigger(WorkflowTimeTrigger trigger) { WorkflowTimeTriggers = WorkflowTimeTriggers.Union(new List<WorkflowTimeTrigger> { trigger }).ToArray(); }
        public void RemoveWorkflowTimeTrigger(string timeTriggerId) { WorkflowTimeTriggers = WorkflowTimeTriggers.Except(WorkflowTimeTriggers.Where(x => x.Id.ToString() == timeTriggerId)).ToArray(); }
        public void AddTransition(WorkflowTransition workflowTransition) { WorkflowTransitions = WorkflowTransitions.Union(new List<WorkflowTransition> { workflowTransition }).ToArray(); }
        public void RemoveTransition(string transitionId) { WorkflowTransitions = WorkflowTransitions.Except(WorkflowTransitions.Where(x => x.Id.ToString() == transitionId)).ToArray(); }
        public void AddWorkflowConfigurationVariable(WorkflowConfigurationVariable workflowConfigurationVariable) { WorkflowConfigurationVariables = WorkflowConfigurationVariables.Union(new List<WorkflowConfigurationVariable> { workflowConfigurationVariable }).ToArray(); }
        public void RemoveWorkflowConfigurationVariable(string workflowConfigurationVariableId) { WorkflowConfigurationVariables = WorkflowConfigurationVariables.Except(WorkflowConfigurationVariables.Where(x => x.Id.ToString() == workflowConfigurationVariableId)).ToArray(); }
        public void ResetIds()
        {
            Id = Guid.NewGuid();
            foreach (var workflowTransition in WorkflowTransitions)
            {
                workflowTransition.ResetIds();
            }
            foreach (var workflowTimeTrigger in WorkflowTimeTriggers)
            {
                workflowTimeTrigger.ResetIds();
            }
            foreach (var workflowConfigurationVariable in WorkflowConfigurationVariables)
            {
                workflowConfigurationVariable.ResetIds();
            }
        }

        public string SerializeToString()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new XmlTextWriter(stream, Encoding.UTF8);
                Serializer.Serialize(writer, this);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static WorkflowConfiguration DeserializeFromString(string serialized)
        {
            using (var reader = new XmlTextReader(new StringReader(serialized)))
            {
                return (WorkflowConfiguration) Serializer.Deserialize(reader);
            }
        }
    }

    [DataContract, Serializable]
    public class WorkflowConfigurationVariable
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        public WorkflowConfigurationVariable()
        {
            Id = Guid.NewGuid();
        }
        public void ResetIds()
        {
            Id = Guid.NewGuid();
        }
    }

    [DataContract, Serializable]
    public class WorkflowTransition
    {
        public WorkflowTransition()
        {
            Id = Guid.NewGuid();
            WorkflowTransitionActionElements = new WorkflowTransitionActionElement[0];
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public WorkflowTransitionActionElement[] WorkflowTransitionActionElements { get; set; }

        public void AddWorkflowTransitionActionElement(WorkflowTransitionActionElement workflowTransitionActionElement)
        {
            if (WorkflowTransitionActionElements.Any(x => x.Name == workflowTransitionActionElement.Name))
            {
                throw new Exception(string.Format("the name [{0}] is already in use under this scope. Names must be unique within the scope", workflowTransitionActionElement.Name));
            }
            WorkflowTransitionActionElements = WorkflowTransitionActionElements.Union(new List<WorkflowTransitionActionElement> {workflowTransitionActionElement}).ToArray();
        }

        public void RemoveWorkflowTransitionActionElement(string workflowTransitionActionElementId) { WorkflowTransitionActionElements = WorkflowTransitionActionElements.Except(WorkflowTransitionActionElements.Where(x => x.Id.ToString() == workflowTransitionActionElementId)).ToArray(); }

        public void ResetIds()
        {
            Id = Guid.NewGuid();
            foreach (var workflowTransitionActionElement in WorkflowTransitionActionElements)
            {
                workflowTransitionActionElement.ResetIds();
            }
        }
    }

    [DataContract, Serializable]
    public class WorkflowTimeTrigger
    {
        public WorkflowTimeTrigger() { Id = Guid.NewGuid(); }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string TransitionToExecute { get; set; }

        [DataMember]
        public string ViewName { get; set; }
        public void ResetIds()
        {
            Id = Guid.NewGuid();
        }
    }



    [DataContract, Serializable]
    public class WorkflowTransitionActionElement
    {
        public WorkflowTransitionActionElement()
        {
            Id = Guid.NewGuid();
            HaltOnFailure = true;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string AssemblyName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public bool HaltOnFailure { get; set; }

        [DataMember]
        public string ExecuteConditionExpression { get; set; }

        [DataMember]
        public string ConfigXml { get; set; }

        public void ResetIds()
        {
            Id = Guid.NewGuid();
        }
    }
}