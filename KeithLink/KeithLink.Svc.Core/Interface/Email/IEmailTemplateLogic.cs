using KeithLink.Svc.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Email
{
	public interface IEmailTemplateLogic
	{
		EmailTemplateModel ReadForKey(string templateKey);
	}
}
