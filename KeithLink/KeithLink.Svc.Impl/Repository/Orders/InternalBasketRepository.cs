using KeithLink.Svc.Core.Interface.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class InternalBasketRepository: IInternalBasketRepository
	{
		/// <summary>
		/// This method should only be called from the internal service. It uses the runtime CS objects,
		/// which require the service to be run on the same server as commerce server.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="basketId"></param>
		/// <returns></returns>
		public string SaveCartAsOrder(Guid userId, Guid basketId)
		{
			var context = CommerceServer.Core.Runtime.Orders.OrderContext.Create(Configuration.CSSiteName);

			var basket = context.GetBasket(userId, basketId);

			var purchaseOrder = basket.SaveAsOrder();

			return purchaseOrder.TrackingNumber;
		}
	}
}
