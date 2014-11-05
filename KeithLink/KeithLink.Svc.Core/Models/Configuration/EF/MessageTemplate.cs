using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Configuration.EF
{
	public enum MessageTemplateType
	{
		Email,
		Push,
		SMS
	}

	public class MessageTemplate: BaseEFModel
	{
		[Index]
		[MaxLength(50)]
		public string TemplateKey { get; set; }
		public string Subject { get; set; }
		public bool IsBodyHtml { get; set; }
		public string Body { get; set; }
		public MessageTemplateType Type { get; set; }
	}
}
