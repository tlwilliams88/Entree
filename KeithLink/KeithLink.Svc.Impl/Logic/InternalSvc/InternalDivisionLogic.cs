using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalDivisionLogic : IInternalDivisionLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IBranchSupportRepository branchSupportRepository;

		public InternalDivisionLogic(IBranchSupportRepository branchSupportRepository, IUnitOfWork unitOfWork)
		{
			this.branchSupportRepository = branchSupportRepository;
			this.unitOfWork = unitOfWork;
		}

		public List<Core.Models.SiteCatalog.BranchSupportModel> ReadBranchSupport()
		{
			var branchSupport = branchSupportRepository.ReadAll();

			if (branchSupport == null)
				return null;
			return branchSupport.Select(b => new BranchSupportModel() {BranchId = b.BranchId, Email = b.Email, SupportPhoneNumber = b.SupportPhoneNumber, TollFreeNumber = b.TollFreeNumber }).ToList();
		}
	}
}
