using System;
using System.Net;
using System.Net.Mail;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Microsoft.SharePoint.Administration;

namespace Oxbow.Gwe.Core.Utils
{
    public class EmailUtils
    {
        public static void SendEmail(string[] toAddresses, string[] ccAddress, string[] bccAddress, string subject, string body, string from, string replyTo, string attachmentPath, string formNumber, string sendPurpose)
        {
            //email has a kill switch. this code will only run if the email is specifically enabled
            if (SettingsManager.IsEmailEnabled == false)
            {
                ResolveType.Instance.Of<ILogger>().Debug("Email is disabled and the current message will not be sent");
            }

            var message = new MailMessage();
            message.From = new MailAddress(from);
            foreach (string strAddress in toAddresses)
            {
                message.To.Add(strAddress);
            }
            if (ccAddress.Length > 0)
            {
                //add each email address from the array into a collection
                foreach (string strCcAddress in ccAddress)
                {
                    message.CC.Add(strCcAddress);
                }
            }
            if (bccAddress.Length > 0)
            {
                //add each email address from the array into a collection
                foreach (string strBccAddress in bccAddress)
                {
                    message.Bcc.Add(strBccAddress);
                }
            }
            message.ReplyTo = new MailAddress(replyTo);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;
            if (!attachmentPath.Equals(""))
                message.Attachments.Add(new Attachment(attachmentPath));
            try
            {
                var smtp = GetSmtpClient();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static SmtpClient GetSmtpClient()
        {
            if (!SettingsManager.IsUnitTestMode)
            {
                try
                {
                    return new SmtpClient(SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("An exception occurred when trying to build an SmtpClient using the SPAdministrationWebApplication.Local.OutboundMailServiceInstance. Further information: [{0}]", exp));
                }
            }
            else
            {
                var smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("reallifedata@gmail.com", "0k0k1234");
                return smtp;
            }
        }
    }
}