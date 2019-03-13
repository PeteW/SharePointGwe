using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Newtonsoft.Json;
using Oxbow.GenericDashboard.Core.Contracts;

namespace Oxbow.GenericDashboard.Core.Models
{
    [Serializable]
    public class GenericDashboardConfiguration
    {
//        private static XmlSerializer _serializer;
        /// <summary>
        /// This is static to avoid deserialization memory leakage.
        /// </summary>
//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if (_serializer == null)
//                {
//                    _serializer = new XmlSerializer(typeof(GenericDashboardConfiguration));
//                }
//                return _serializer;
//            }
//        }

        public List<TabConfiguration> TabConfigurations { get; set; }
        public List<JumpMenuItem> JumpMenuItems { get; set; }
        public void RemoveJumpMenuItem(Guid id) { JumpMenuItems = JumpMenuItems.Where(x => x.Id != id).ToList(); }
        public string JumpMenuPrompt { get; set; }
        public JumpMenuItem AddJumpMenuItem()
        {
            var result = new JumpMenuItem();
            result.Id = Guid.NewGuid();
            result.OrderId = JumpMenuItems.Count + 1;
            result.Name = "Untitled";
            result.Url = "http://";
            result.Target = "_self";
            JumpMenuItems.Add(result);
            return result;
        }
        public TabConfiguration AddTabConfiguration()
        {
            var result = new TabConfiguration();
            result.Id = Guid.NewGuid();
            result.IsCountDisplayed = true;
            result.ListName = "List";
            result.Name = "Untitled";
            result.OrderId = TabConfigurations.Count + 1;
            result.PageSize = 25;
            result.ViewName = "view";
            result.WebUrl = "http://";
            TabConfigurations.Add(result);
            return result;
        }
        public void RemoveTabConfiguration(Guid id) { TabConfigurations = TabConfigurations.Where(x => x.Id != id).ToList(); }

        public GenericDashboardConfiguration()
        {
            TabConfigurations = new List<TabConfiguration>();
            JumpMenuItems = new List<JumpMenuItem>();
        }

        public string Dehydrate()
        {
//            using (var stream = new MemoryStream())
//            {
//                var writer = new XmlTextWriter(stream, Encoding.UTF8);
//                Serializer.Serialize(writer, this);
//                writer.Flush();
//                stream.Seek(0, SeekOrigin.Begin);
//                using (var streamReader = new StreamReader(stream, Encoding.UTF8))
//                {
//                    return streamReader.ReadToEnd();
//                }
//            }
            return JsonConvert.SerializeObject(this);
        }

        public static GenericDashboardConfiguration Hydrate(string serialized)
        {
//            using (var reader = new XmlTextReader(new StringReader(serialized)))
//            {
//                return (GenericDashboardConfiguration)Serializer.Deserialize(reader);
//            }
            return JsonConvert.DeserializeObject<GenericDashboardConfiguration>(serialized);
        }
    }
    [Serializable]
    public class JumpMenuItem
    {
        public Guid Id{ get; set;}
        public string Name { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public int OrderId { get; set; }
    }
    [Serializable]
    public class TabConfiguration
    {
        public Guid Id{ get; set;}
        public string Name { get; set; }
        public int OrderId { get; set; }
        public string WebUrl { get; set; }
        public string ListName { get; set; }
        public string ViewName { get; set; }
        public bool IsCountDisplayed { get; set; }
        public int PageSize { get; set; }
        public List<RowCustomAction> RowCustomActions { get; set; }

        public TabConfiguration()
        {
            RowCustomActions = new List<RowCustomAction>();
        }

        public RowCustomAction AddRowCustomAction()
        {
            var result = new RowCustomAction();
            result.Id = Guid.NewGuid();
            result.Name = "CustomAction_"+(RowCustomActions.Count + 1).ToString();
            result.AssemblyName = "Emc.It4me.Commons, Culture=Neutral, Version=1.0.0.0, PublicKeyToken=bcc485d73486e5e7";
            result.TypeName = "Emc.It4me.Commons.GenericDashboardIntegration.ClaimTaskCustomActionHandler";
            //commented by vani
            //result.AssemblyName = "Oxbow.GenericDashboard.Core, Culture=Neutral, Version=1.0.0.0, PublicKeyToken=8168d5868e89fd50";
            //result.TypeName = "Oxbow.GenericDashboard.Core.Utils.DummyRowCustomActionHandler";
            RowCustomActions.Add(result);
            return result;
        }
        public void RemoveRowCustomAction(Guid id)
        {
            RowCustomActions = RowCustomActions.Where(x => x.Id != id).ToList();
        }
    }
    [Serializable]
    public class RowCustomAction
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        
        /// <summary>
        /// cache the assemblies to prevent memory leakage
        /// </summary>
        private static List<Assembly> _assemblies;
 
        public void Execute(SPListItem spListItem,string ActionName)
        {
            Type type;
            try
            {
                if (string.IsNullOrEmpty(TypeName) || string.IsNullOrEmpty(AssemblyName))
                    throw new Exception(string.Format("For custom action named [{0}], the assembly and/or type name are blank.", Name));
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
            catch (Exception exp)
            {
                throw new Exception(string.Format("The following exception occurred when trying to load the custom type [{0}] assembly [{1}]: {2}", TypeName, AssemblyName, exp));
            }
            if (!(typeof(IRowCustomActionHandler).IsAssignableFrom(type)))
                throw new Exception(string.Format("The specified type [{0}] assembly [{1}] was successfully loaded but did not implement the interface [{2}]", TypeName, AssemblyName, typeof(IRowCustomActionHandler)));
            try
            {
                var instance = Activator.CreateInstance(type);
                ((IRowCustomActionHandler)instance).Execute(spListItem,ActionName);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("The following exception occurred within the custom action of type [{0}] assembly [{1}] on list item [{2}]: {3}", TypeName, AssemblyName, spListItem.Url, exp));
            }
        }
    }
}