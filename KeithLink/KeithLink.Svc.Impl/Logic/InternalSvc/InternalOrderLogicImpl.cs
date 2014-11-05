using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalOrderLogicImpl: IInternalOrderLogic
	{
		private readonly IOrderHistoryHeaderRepsitory orderHistoryRepository;
		private readonly IUnitOfWork unitOfWork;

		public InternalOrderLogicImpl(IOrderHistoryHeaderRepsitory orderHistoryRepository, IUnitOfWork unitOfWork)
		{
			this.orderHistoryRepository = orderHistoryRepository;
			this.unitOfWork = unitOfWork;
		}

		public DateTime? ReadLatestUpdatedDate(Core.Models.SiteCatalog.UserSelectedContext catalogInfo)
		{
			var orders = orderHistoryRepository.Read(o => o.CustomerNumber.Equals(catalogInfo.CustomerId, StringComparison.InvariantCultureIgnoreCase) &&
				o.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase));

			if (orders.Any())
				return orders.Max(m => m.ModifiedUtc);
			else
				return null;
		}
	}
}
