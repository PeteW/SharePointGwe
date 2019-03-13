using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    public abstract class WorkflowTransitionAction
    {
        private static Dictionary<Type, XmlSerializer> _serializers;
        [XmlIgnore]
        public EventHandler<EventArgs> OnSaveRequested { get; set; } 

        private static XmlSerializer GetXmlSerializer(Type t)
        {
            if (_serializers == null)
                _serializers = new Dictionary<Type, XmlSerializer>();
            if (!_serializers.ContainsKey(t))
                _serializers.Add(t, new XmlSerializer(t));
            return _serializers[t];
        }
        [XmlIgnore]
        public WorkflowConfiguration WorkflowConfiguration { get; set; }
        protected WorkflowTransitionAction(WorkflowConfiguration workflowConfiguration)
        {
            WorkflowConfiguration = workflowConfiguration;
        }
        protected WorkflowTransitionAction()
        {
        }


        public abstract UserControl GenerateUserControl(Page page);
        public abstract void UpdateFromUserControl(UserControl userControl);
        public abstract string GetTypeName();

        public WorkflowTransitionAction Deserialize(string xml)
        {
            using (var reader = new XmlTextReader(new StringReader(xml)))
            {
                return (WorkflowTransitionAction) GetXmlSerializer(GetType()).Deserialize(reader);
            }
        }

        public abstract void Execute(SPListItem spListItem);

        public string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.OmitXmlDeclaration = true;
                using (var writer = XmlTextWriter.Create(stream, settings))
                {
                    GetXmlSerializer(GetType()).Serialize(writer, this);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }                    
                }
            }
        }
    }
}