using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile.PasswordReset
{
	public class PasswordResetRequestRepositoryImpl: EFBaseRepository<PasswordResetRequest>, IPasswordResetRequestRepository
	{
		public PasswordResetRequestRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        
	}
}
