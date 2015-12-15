using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Models.SpecialOrders.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SpecialOrders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Data.SqlClient;


namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class SpecialOrderRepositoryImpl : ISpecialOrderRepository
    {
        #region attributes
        private ISpecialOrderDBContext _specialOrderDbContext;
        private IOrderHistoryHeaderRepsitory _orderHistory;
        private IOrderHistoryDetailRepository _orderHistoryDetailRepo;
        private IUnitOfWork _unitOfWork;
        
        #endregion

        #region ctor
        public SpecialOrderRepositoryImpl(ISpecialOrderDBContext specialOrderDbContext, IOrderHistoryHeaderRepsitory orderHistory, 
            IOrderHistoryDetailRepository orderHistoryDetailRepo, IUnitOfWork unitOfWork)
        {
			_specialOrderDbContext = specialOrderDbContext;
            _orderHistory = orderHistory;
            _orderHistoryDetailRepo = orderHistoryDetailRepo;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region methods

		
        #endregion

		public void Create(Core.Models.Orders.OrderFile header)
		{

			var query = _specialOrderDbContext.RequestHeaderIds
                .SqlQuery("dbo.spGetNextRequestHeaderId @branchId", new SqlParameter("branchId", header.Header.Branch));
            string idToUse = query.FirstAsync().Result.CurrentId;

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
                var item = new RequestItem()
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
				};
				_specialOrderDbContext.RequestItems.Add(item);               
			}

			_specialOrderDbContext.Context.SaveChanges();

            // add idToUse to order history
            var orderHistory = _orderHistory.ReadByConfirmationNumber(header.Header.ControlNumber.ToString().PadLeft(7, '0'), "B").First(); // TODO, use constant for source
            //details
            foreach (var orderItem in orderHistory.OrderDetails)
            {
                var detailItems = header.Details.Where(x => x.ItemNumber == orderItem.ItemNumber).ToList();
                foreach (var detailItem in detailItems)
                {
                    orderItem.ManufacturerId = detailItem.ManufacturerName;//todo
                    orderItem.SpecialOrderLineNumber = detailItem.LineNumber.ToString();
                    orderItem.SpecialOrderHeaderId = idToUse.ToString().PadLeft(7, '0');
                }
            }
            _unitOfWork.SaveChangesAndClearContext();

            requestHeader.OrderStatusId = "05";
            _specialOrderDbContext.Context.SaveChanges();
		}
	}
}
