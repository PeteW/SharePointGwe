using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    [DataContract]
    public class EmailWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string TargetRecipients { get; set; }
        [DataMember]
        public string TargetCc { get; set; }
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Body { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<EmailWorkflowTransitionAction>)page.LoadControl("~/_controltemplates/Oxbow.Gwe/EmailWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl)result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<EmailWorkflowTransitionAction>)userControl).UpdateDataModel(this); }
        public override string GetTypeName() { return "Email"; }
        public override void Execute(SPListItem spListItem)
        {
            if (string.IsNullOrEmpty(TargetRecipients) || string.IsNullOrEmpty(From))
            {
                ResolveType.Instance.Of<ILogger>().Error("Required fields were missing from the email action. Execution is quietly aborted.");
                return;
            }
            if (SettingsManager.IsEmailEnabled == false)
                throw new Exception("Sending a test email is going to fail because email is currently disabled.");
            var container = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration);
            var targetRecipients = ResolveType.Instance.Of<ITemplateEngine>().Render(TargetRecipients, container).Split(new char[] { ',' }).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var targetCc = ResolveType.Instance.Of<ITemplateEngine>().Render((TargetCc ?? string.Empty), container).Split(new char[] { ',' }).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var from = ResolveType.Instance.Of<ITemplateEngine>().Render(From, container);
            var subject = ResolveType.Instance.Of<ITemplateEngine>().Render(Subject, container);
            var body = ResolveType.Instance.Of<ITemplateEngine>().Render(Body, container);

            EmailUtils.SendEmail(
                targetRecipients,
                targetCc,
                new string[] { },
                subject,
                body,
                from,
                from,
                string.Empty,
                "none",
                "SendTestEmail"
                );
            
        }
    }
}