using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Email
{
	public interface IMessageTemplateLogic
	{
		MessageTemplateModel ReadForKey(string templateKey);
        StringBuilder BuildHeader(string subject, Customer customer);
    }
}
