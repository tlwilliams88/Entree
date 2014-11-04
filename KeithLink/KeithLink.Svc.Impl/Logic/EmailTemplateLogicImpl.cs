using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic
{
	public class EmailTemplateLogicImpl: IEmailTemplateLogic
	{
		private readonly IEmailTemplateRepository emailTemplateRepository;

		public EmailTemplateLogicImpl(IEmailTemplateRepository emailTemplateRepository)
		{
			this.emailTemplateRepository = emailTemplateRepository;
		}


		public EmailTemplateModel ReadForKey(string templateKey)
		{
			var template = emailTemplateRepository.Read(t => t.TemplateKey.Equals(templateKey)).FirstOrDefault();

			if (template == null)
				return null;

			return new EmailTemplateModel() { Id = template.Id, Body = template.Body, IsBodyHtml = template.IsBodyHtml, ModifiedOn = template.ModifiedUtc, Subject = template.Subject, TemplateKey = template.TemplateKey };
		}
	}
}
