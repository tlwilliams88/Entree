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
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl
{
    public class EmailClientImpl: IEmailClient
	{
		#region Properties
        private IEventLogRepository _log;
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

		public EmailClientImpl(IEventLogRepository log)
        {
            this.serviceEmailAddress = Configuration.ServiceEmailAddress;
			this.smtpHostName = Configuration.SMTPHostName;
			this.smtpSendPort = Configuration.SMTPSendPort;
			this.smtpUserName = Configuration.SMTPUsername;
			this.smtpPassword = Configuration.SMTPPassword;
            _log = log;
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

            if (body.IndexOf("|LOGO|") > -1) // If the logo will be in this email (most notifications) pull its stream from the embedded resource and create an alternate view on the mailmessage
            {
                body = body.Replace("|LOGO|", "<img src=\"cid:LOGO\" alt=\"BEK\" />");
                message.Body = body;
                Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
                Stream logoStream = assembly.GetManifestResourceStream("KeithLink.Svc.Impl.Images.Logo.png");
                var inlineLogo = new LinkedResource(logoStream);
                inlineLogo.ContentId = "LOGO";
                //if(logoStream != null && logoStream.Length>0) _log.WriteInformationLog("EmailClientImpl logo image found.");
                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                view.LinkedResources.Add(inlineLogo);
                message.AlternateViews.Add(view);
            }

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

            try {
                client.Send( message );
                _log.WriteInformationLog(string.Format(" Sending email to {0}", string.Join(",", toAddresses.ToArray())));
            } catch (Exception e) {
                throw e;                
            } finally {
                client.Dispose();
            }
        }
				
	}
}
