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
	public class MessageTemplateRepositoryImpl: EFBaseRepository<MessageTemplate>, IMessageTemplateRepository
	{
		public MessageTemplateRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        
	}
}
