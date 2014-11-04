using KeithLink.Svc.Core.Models.Configuration.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Email
{
	public interface IEmailTemplateRepository: IBaseEFREpository<EmailTemplate>
	{
	}
}
