using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Email
{
	public class EmailTemplateRepositoryImpl: EFBaseRepository<EmailTemplate>, IEmailTemplateRepository
	{
		public EmailTemplateRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        
	}
}
