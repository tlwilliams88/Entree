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
    public class SpecialOrderRepositoryImpl : ISpecialOrderRepository
    {
        #region attributes
        private ISpecialOrderDBContext _specialOrderDbContext;
        #endregion

        #region ctor
		public SpecialOrderRepositoryImpl(ISpecialOrderDBContext specialOrderDbContext)
        {
			_specialOrderDbContext = specialOrderDbContext;
        }
        #endregion

        #region methods

		
        #endregion

		public void Create(Core.Models.Orders.OrderFile header)
		{
			var query = _specialOrderDbContext.RequestHeaderIds
                .SqlQuery("dbo.spGetNextRequestHeaderId");
			long idToUse = -1;
			foreach (var result in query) // will only be one
				idToUse = result.CurrentId;			

			// next, call create after converting OrderFile to RequestHeader
			_specialOrderDbContext.RequestHeaders.Add(new RequestHeader()
			{
				// map in all possible fields here
				RequestHeaderId = idToUse.ToString(),
				BranchId = header.Header.Branch,
				CategoryId = 0, // need to get the category id
				DsrNumber = "", // need to feed the dsr number forward?  or look it up?
				CustomerNumber = header.Header.CustomerNumber,
				// do we need address info????
				// do we need a 'contact'????
				ManufacturerName = "UNFI", // just use UNFI????
				OrderStatusId = "00", // New
				ShipMethodId = 3, // Drop Ship
				UpdatedBy = "Entree" // how to get this user????
			});
			foreach (var detail in header.Details)
			{
				_specialOrderDbContext.RequestItems.Add(new RequestItem()
				{
					// map in all possible fields here
					RequestHeaderId = idToUse.ToString(),
					BranchId = header.Header.Branch,
					LineNumber = (byte)detail.LineNumber,
					OrderStatusId = "00", // New,
					ShipMethodId = 3, // Drop Ship
					ManufacturerNumber = detail.ItemNumber,
					Price = (float)detail.SellPrice,
					PONumber = header.Header.PONumber
				});
			}

			_specialOrderDbContext.Context.SaveChanges();
		}
	}
}
