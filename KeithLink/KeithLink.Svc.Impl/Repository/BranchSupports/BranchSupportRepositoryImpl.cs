
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.EF;
using System.Data.Entity;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Repository.BranchSupports
{
	public class BranchSupportRepositoryImpl : EFBaseRepository<BranchSupport>, IBranchSupportRepository
	{
		public BranchSupportRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
				
	}
}
