using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Configuration
{
	public class MessageTemplateModel
	{
		public long Id { get; set; }
		public string TemplateKey { get; set; }
		public string Subject { get; set; }
		public bool IsBodyHtml { get; set; }
		public string Body { get; set; }
		public DateTime ModifiedOn { get; set; }
	}
}
