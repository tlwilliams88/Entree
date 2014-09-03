using CommerceServer.Core.Runtime.Orders;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OrderService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select OrderService.svc or OrderService.svc.cs at the Solution Explorer and start debugging.
	public class OrderService : IOrderService
	{
		private readonly IInternalBasketRepository internalBasketRepository;

		public OrderService(IInternalBasketRepository internalBasketRepository)
		{
			this.internalBasketRepository = internalBasketRepository;
		}

		public string SaveCartAsOrder(Guid userId, Guid cartId)
		{
			return internalBasketRepository.SaveCartAsOrder(userId, cartId);
		}
	}
}
