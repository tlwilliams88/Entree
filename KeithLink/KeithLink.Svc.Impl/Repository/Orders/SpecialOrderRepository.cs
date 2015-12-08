using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Models.SpecialOrders.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.SpecialOrders;
using KeithLink.Svc.Core.Interface.Profile;

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
            long idToUse = query.FirstAsync().Result.CurrentId;

			// next, call create after converting OrderFile to RequestHeader
            var requestHeader = new RequestHeader()
            {
                RequestHeaderId = idToUse.ToString().PadLeft(7, '0'),
                BranchId = header.Header.Branch,
                CategoryId = 9, // need to get the category id
                CustomerNumber = header.Header.CustomerNumber,
                DsrNumber = header.Header.DsrNumber, // do we need to feed the dsr number forward?  or look it up?
                Address = header.Header.AddressStreet,
                City = header.Header.AddressCity,
                State = header.Header.AddressRegionCode,
                Zip = header.Header.AddressPostalCode,// do we need address info????
                // do we need a 'contact'????
                ManufacturerName = "UNFI", // just use UNFI????
                OrderStatusId = "00", // New
                ShipMethodId = 0, // Auto Release
                UpdatedBy = "Entree", // how to get this user????
                Source = header.Header.CatalogType
            };
			_specialOrderDbContext.RequestHeaders.Add(requestHeader);

            _specialOrderDbContext.Context.SaveChanges();

			foreach (var detail in header.Details)
			{
				_specialOrderDbContext.RequestItems.Add(new RequestItem()
				{
					RequestHeaderId = idToUse.ToString().PadLeft(7, '0'),
					BranchId = header.Header.Branch,
					LineNumber = (byte)detail.LineNumber,
					OrderStatusId = "00", // New,
					ShipMethodId = 0, // Auto Release Sep Inv
					ManufacturerNumber = detail.ItemNumber,
					Price = (float)detail.SellPrice,
					PONumber = header.Header.PONumber,
                    Quantity = (Byte) detail.OrderedQuantity,
                    Description = detail.Description,
                    Comments = detail.ManufacturerName
				});
			}

			_specialOrderDbContext.Context.SaveChanges();

            requestHeader.OrderStatusId = "05";

            _specialOrderDbContext.Context.SaveChanges();
		}
	}
}
