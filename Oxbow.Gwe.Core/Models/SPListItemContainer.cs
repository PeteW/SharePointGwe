using System;
using System.Text;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;

namespace Oxbow.Gwe.Core.Models
{
    public class SpListItemContainer:ISpListItemContainer
    {
        private SPListItem _spListItem;

        public SPListItem SpListItemStrict
        {
            get
            {
                if (_spListItem == null)
                    throw new Exception("Cannot get SPListItem because this SpListItemContainer was initialized without using an SPListItem");
                return _spListItem;
            }
            set { _spListItem = value; }
        }

        public SPListItem SpListItem
        {
            get { return _spListItem; }
            set { _spListItem = value; }
        }

        public SpListItemContainer(){}

        public SpListItemContainer(SPListItem spListItem, WorkflowConfiguration workflowConfiguration)
        {
            _spListItem = spListItem;
            WorkflowConfiguration = workflowConfiguration;
        }

        public WorkflowConfiguration WorkflowConfiguration { get; set; }

        public WorkflowConfiguration WorkflowConfigurationStrict
        {
            get
            {
                if (WorkflowConfiguration == null)
                    throw new Exception("Cannot get WorkflowConfiguration because this SPListItemContainer was initialized without using a WorkflowConfiguration");
                return WorkflowConfiguration;
            }
        }

        public virtual void PerformSystemSave()
        {
            _spListItem.SystemUpdate(false);
        }
    }
}