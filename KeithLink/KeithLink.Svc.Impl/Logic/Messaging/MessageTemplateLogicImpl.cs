using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
	public class MessageTemplateLogicImpl: IMessageTemplateLogic
	{
		private readonly IMessageTemplateRepository emailTemplateRepository;

		public MessageTemplateLogicImpl(IMessageTemplateRepository emailTemplateRepository)
		{
			this.emailTemplateRepository = emailTemplateRepository;
		}


		public MessageTemplateModel ReadForKey(string templateKey)
		{
			var template = emailTemplateRepository.Read(t => t.TemplateKey.Equals(templateKey)).FirstOrDefault();

			if (template == null)
				return null;

			return new MessageTemplateModel() { Id = template.Id, Body = template.Body, IsBodyHtml = template.IsBodyHtml, ModifiedOn = template.ModifiedUtc, Subject = template.Subject, TemplateKey = template.TemplateKey };
		}
	}
}
