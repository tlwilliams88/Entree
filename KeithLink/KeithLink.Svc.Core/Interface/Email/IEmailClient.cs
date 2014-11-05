using KeithLink.Svc.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Email

{
	public interface IEmailClient
	{
		void SendTemplateEmail(MessageTemplateModel template, List<string> emails, List<string> ccAddresses, List<string> bccAddresses, object tokens);
		void SendTemplateEmail(MessageTemplateModel template, List<string> emails, object tokens);
		void SendEmail(List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string subject, string body, bool isBodyHtml = false);
		void SendEmail(List<string> toAddresses, List<string> ccAddresses, List<string> bccAddresses, string fromAddress, string subject, string body, bool isBodyHtml = false);
	}
}
