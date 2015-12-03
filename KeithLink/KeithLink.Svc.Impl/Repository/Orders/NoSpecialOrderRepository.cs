using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Models.SpecialOrders.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.SpecialOrders;

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;


namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class NoSpecialOrderRepositoryImpl : ISpecialOrderRepository
    {

        #region ctor
		public NoSpecialOrderRepositoryImpl()
        {
        }
        #endregion

		public void Create(Core.Models.Orders.OrderFile header)
		{
		}
	}
}
