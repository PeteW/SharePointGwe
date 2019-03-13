using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    public class WorkflowTransitionActionFactory
    {
        //todo: this should be configurable from the root level.

        private static List<Assembly> _assemblies;

        public static IEnumerable<KeyValuePair<string, string>> AvailableTypes
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                           {
                               new KeyValuePair<string, string>("List Logging", "Oxbow.Gwe.Core.WorkflowTransitionActions.ListLogWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("Email", "Oxbow.Gwe.Core.WorkflowTransitionActions.EmailWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("Custom Code", "Oxbow.Gwe.Core.WorkflowTransitionActions.CustomCodeWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("Form Editor", "Oxbow.Gwe.Core.WorkflowTransitionActions.FormEditorWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("List Item Editor", "Oxbow.Gwe.Core.WorkflowTransitionActions.ListItemEditorWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("Permission set", "Oxbow.Gwe.Core.WorkflowTransitionActions.PermissionSetWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c"),
                               new KeyValuePair<string, string>("Inject XML", "Oxbow.Gwe.Core.WorkflowTransitionActions.InjectXmlWorkflowTransitionAction|Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c")
                           };
            }
        }

        public static WorkflowTransitionAction CreateNewWorkflowTransitionAction(string typeName, string assemblyName)
        {
            if (_assemblies == null)
                _assemblies = new List<Assembly>();
            Assembly assembly = _assemblies.Where(x => x.FullName == assemblyName).FirstOrDefault();
            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
                _assemblies.Add(assembly);
            }
            Type type = assembly.GetType(typeName, true);
            return (WorkflowTransitionAction)Activator.CreateInstance(type);
        }

        public static WorkflowTransitionAction[] GetWorkflowTransitionActions(WorkflowTransition workflowTransition, WorkflowConfiguration workflowConfiguration) { return workflowTransition.WorkflowTransitionActionElements.OrderBy(x => x.OrderId).Select(x => GetWorkflowTransitionAction(x, workflowConfiguration)).ToArray(); }

        public static WorkflowTransitionAction GetWorkflowTransitionAction(WorkflowTransitionActionElement workflowTransitionActionElement, WorkflowConfiguration workflowConfiguration)
        {
            try
            {
                workflowTransitionActionElement.ConfigXml = (workflowTransitionActionElement.ConfigXml ?? string.Empty).Replace("&lt;?xml version=\"1.0\" encoding=\"utf-8\"?&gt;", "");
                workflowTransitionActionElement.ConfigXml = (workflowTransitionActionElement.ConfigXml ?? string.Empty).Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                WorkflowTransitionAction instance = CreateNewWorkflowTransitionAction(workflowTransitionActionElement.TypeName, workflowTransitionActionElement.AssemblyName);
                var result = instance.Deserialize(workflowTransitionActionElement.ConfigXml);
                result.WorkflowConfiguration = workflowConfiguration;
                return result;
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("There was an error reading the following ConfigXml from the WorkflowTransitionElement [{0}]. ConfigXml: [{1}] Exception: [{2}]", workflowTransitionActionElement.Name, workflowTransitionActionElement.ConfigXml, exp));
            }
        }
    }
}