using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowEngine
{
    public class MailWorkflowTransitionHandler
    {
        public static void SendWorkflowErrorMessage(SPListItem spListItem, WorkflowConfiguration workflowConfiguration, Exception exception)
        {
            
            EmailTemplateReader emailTemplateReader = EmailTemplateReader.CreateTemplateFromResource("Oxbow.Gwe.Core.Configuration.AdministratorWorkflowError.template", new Dictionary<string, string>
                                                                                                                                                                             {
                                                                                                                                                                                 {"listItemId", spListItem.ID.ToString()},
                                                                                                                                                                                 {"listItemUrl", spListItem.GetAbsoluteUrl()},
                                                                                                                                                                                 {"exception", exception.ToString()},
                                                                                                                                                                                 {"from", workflowConfiguration.AdminFromEmail},
                                                                                                                                                                                 {"to", workflowConfiguration.AdminToEmail},
                                                                                                                                                                                 {"cc", workflowConfiguration.AdminCcEmail}
                                                                                                                                                                             });
            EmailUtils.SendEmail(
                emailTemplateReader.TargetRecipients.ToArray(),
                emailTemplateReader.TargetCC.ToArray(),
                new string[] {},
                emailTemplateReader.Subject,
                emailTemplateReader.Body,
                emailTemplateReader.From,
                emailTemplateReader.From,
                string.Empty,
                "none",
                "SendErrorEmail"
                );
        }
    }
}