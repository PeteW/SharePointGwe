using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    [DataContract]
    public class CustomCodeWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public string AssemblyName { get; set; }

        private static List<Assembly> _assemblies;

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<CustomCodeWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/CustomCodeWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<CustomCodeWorkflowTransitionAction>)userControl).UpdateDataModel(this); }
        public override string GetTypeName() { return "Custom Code"; }
        public override void Execute(SPListItem spListItem)
        {
            Type type;
            try
            {
                if (string.IsNullOrEmpty(TypeName) || string.IsNullOrEmpty(AssemblyName))
                    return;
                if (_assemblies == null)
                    _assemblies = new List<Assembly>();
                var assembly = _assemblies.Where(x => x.FullName == AssemblyName).FirstOrDefault();
                if (assembly == null)
                {
                    assembly = Assembly.Load(AssemblyName);
                    _assemblies.Add(assembly);
                }
                type = assembly.GetType(TypeName, true);
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("The following exception occurred when trying to load the custom type [{0}] assembly [{1}]: {2}", TypeName, AssemblyName, exp));
            }
            if(!(typeof(IWorkflowTransitionCustomAction).IsAssignableFrom(type)))
                throw new Exception(string.Format("The specified type [{0}] assembly [{1}] was successfully loaded but did not implement the interface [{2}]", TypeName, AssemblyName, typeof(IWorkflowTransitionCustomAction)));
            try
            {
                var instance = Activator.CreateInstance(type);
                ((IWorkflowTransitionCustomAction)instance).Execute(spListItem);
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("The following exception occurred within the custom action of type [{0}] assembly [{1}] on list item [{2}]: {3}", TypeName, AssemblyName, spListItem.Url, exp));
            }
        }
    }
}