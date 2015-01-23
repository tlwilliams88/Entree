using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl
{
    public class EmailClientImpl: IEmailClient
	{
		#region Properties
		private string serviceEmailAddress;
		private string smtpHostName { get; set; }
		private int smtpSendPort { get; set; }
		private string smtpPassword { get; set; }
		private string smtpUserName { get; set; }
		private string filePathEmailTemplates { get; set; }
		private string filePathGlobalTokens { get; set; }
		

		private SmtpClient _smtpClient;
		public SmtpClient smtpClient
		{
			get
			{
				_smtpClient = new SmtpClient(this.smtpHostName, this.smtpSendPort);
				return _smtpClient;
			}
			set { smtpClient = value; }
		}

		#endregion

		public EmailClientImpl()
        {
            this.serviceEmailAddress = Configuration.ServiceEmailAddress;
			this.smtpHostName = Configuration.SMTPHostName;
			this.smtpSendPort = Configuration.SMTPSendPort;
			this.smtpUserName = Configuration.SMTPUsername;
			this.smtpPassword = Configuration.SMTPPassword;

        }

		public void SendTemplateEmail(MessageTemplateModel template, List<string> emails, List<string> ccAddresses, List<string> bccAddresses, object tokens)
		{
            this.SendEmail(
                emails.ToList(),
                ccAddresses != null ? ccAddresses.ToList() : null,
                bccAddresses != null ? bccAddresses.ToList() : null,
                template.Subject.Inject(tokens),
                template.Body.Inject(tokens),
                template.IsBodyHtml
                );
		}

		public void SendTemplateEmail(MessageTemplateModel template, List<string> emails, object tokens)
        {
			SendTemplateEmail(template, emails, null, null, tokens);
        }

		public void SendEmail(List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string subject, string body, bool isBodyHtml = false)
        {
			this.SendEmail(toAddresses, ccAddresses, bccAddresses, this.serviceEmailAddress, subject, body, isBodyHtml);
        }

		public void SendEmail(List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string fromAddress, string subject, string body, bool isBodyHtml = false)
        {
			if (string.IsNullOrEmpty(fromAddress))
                throw new Exception("Could not send email because property serviceEmailAddress is not defined.");

            if (string.IsNullOrEmpty(this.smtpHostName))
                throw new Exception("Could not send email because property smtpHostName is not defined.");

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            foreach (string toAddress in toAddresses)
                message.To.Add(new MailAddress(toAddress));

            if (ccAddresses != null)
            {
                foreach (string ccAddress in ccAddresses)
                    message.CC.Add(new MailAddress(ccAddress));
            }

			if (bccAddresses != null)
			{
				foreach (string bccAddress in bccAddresses)
					message.Bcc.Add(new MailAddress(bccAddress));
			}

            SmtpClient client = this.smtpClient;

            if (!string.IsNullOrEmpty(this.smtpUserName) && !string.IsNullOrEmpty(this.smtpPassword))
            {
                client.Credentials = new NetworkCredential(this.smtpUserName, this.smtpPassword);
                client.UseDefaultCredentials = false;
            }

            try
            {
                client.Send(message);
			}
            finally
            {
                client.Dispose();
            }
        }
				
	}
}
