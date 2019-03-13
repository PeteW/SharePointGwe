using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    public class InjectXmlWorkflowTransitionAction:WorkflowTransitionAction
    {
        [DataMember]
        public string TargetFieldXpath { get; set; }

        [DataMember]
        public string Xml { get; set; }

        [DataMember]
        public bool PrependXml { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<InjectXmlWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/InjectXmlWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl)
        {
            ((IWorkflowTransitionActionConfigControl<InjectXmlWorkflowTransitionAction>)userControl).UpdateDataModel(this);
        }

        public override string GetTypeName()
        {
            return "Inject XML";
        }

        public override void Execute(SPListItem spListItem)
        {
            var formContainer = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration) as FormContainer;
            if (formContainer == null)
                throw new Exception(spListItem.GetDescription() + " : Expected to have a type IFormContainer.");
            try
            {
                var templateEngine = ResolveType.Instance.Of<ITemplateEngine>();
                var xpath = templateEngine.Render(TargetFieldXpath, formContainer);
                var xml = templateEngine.Render(Xml, formContainer);
                formContainer.AppendChildXml(xpath, PrependXml, xml);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Error with Injecting XML [{0}] into the path [{2}]: {1}", Xml, exp, TargetFieldXpath));
            }
            try
            {
                Thread.Sleep(2000);
                formContainer.PerformSystemSave();
                Thread.Sleep(2000);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("Error when saving the list item [{0}]: {1}", spListItem.Title, exp));
            }
        }
    }
}