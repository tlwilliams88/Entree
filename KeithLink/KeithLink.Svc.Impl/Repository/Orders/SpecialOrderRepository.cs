using KeithLink.Svc.Core;

using KeithLink.Svc.Core.Extensions.Enumerations;

using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SpecialOrders;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SpecialOrders.EF;

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

        private const int CONFNUMBER_LENGTH = 7;
        private const char CONFNUMBER_PADDINGCHAR = '0';

        private const int KSOS_CATEGORYID = 9;
        private const string KSOS_MFGNAME = "UNFI";
        private const int KSOS_SHIPMETHOD_AUTORELEASE = 0;
        private const string KSOS_STATUS_NEW = "00";
        private const string KSOS_STATUS_SEND = "05";
        private const int KSOS_USERID_LENGTH = 50;
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
        private RequestItem BuildRequestDetail(OrderDetail detail, string requestHeaderId, string branchId,
                                                                  string customerNumber, string poNumber) {
            RequestItem item = new RequestItem();

            item.RequestHeaderId = requestHeaderId;
            item.BranchId = branchId;
            item.LineNumber = (byte)detail.LineNumber;
            item.OrderStatusId = KSOS_STATUS_NEW;
            item.ShipMethodId = KSOS_SHIPMETHOD_AUTORELEASE;
            item.ManufacturerNumber = detail.ItemNumber;
            item.EstimateCost = (float)detail.UnitCost;
            item.Price = (float)detail.SellPrice;
            item.PONumber = poNumber;
            item.Quantity = (Byte)detail.OrderedQuantity;
            item.Description = detail.Description;
            item.Comments = detail.ManufacturerName;
            item.UnitOfMeasure = detail.UnitOfMeasure.ToLongString();

            return item;
        }

        private RequestHeader BuildRequestHeader(OrderHeader header, string requestHeaderId) {
            RequestHeader requestHeader = new RequestHeader();

            requestHeader.RequestHeaderId = requestHeaderId;
            requestHeader.BranchId = header.Branch;
            requestHeader.CategoryId = KSOS_CATEGORYID;
            requestHeader.CustomerNumber = header.CustomerNumber;
            requestHeader.DsrNumber = header.DsrNumber;
            requestHeader.Address = header.AddressStreet;
            requestHeader.City = header.AddressCity;
            requestHeader.State = header.AddressRegionCode;
            requestHeader.Zip = header.AddressPostalCode;
            // do we need a 'contact'????
            requestHeader.ManufacturerName = KSOS_MFGNAME;
            requestHeader.OrderStatusId = KSOS_STATUS_NEW;
            requestHeader.ShipMethodId = KSOS_SHIPMETHOD_AUTORELEASE;
            requestHeader.UpdatedBy = header.UserId.Length > KSOS_USERID_LENGTH ? header.UserId.Substring(0, KSOS_USERID_LENGTH) : header.UserId;
            requestHeader.Source = header.CatalogType;

            return requestHeader;
        }

        public void Create(OrderFile file) {
            var query = _specialOrderDbContext.RequestHeaderIds
                .SqlQuery("dbo.spGetNextRequestHeaderId @branchId", new SqlParameter("branchId", file.Header.Branch));
            string headerId = query.FirstAsync().Result.CurrentId.PadLeft(CONFNUMBER_LENGTH, CONFNUMBER_PADDINGCHAR);

            // next, call create after converting OrderFile to RequestHeader
            RequestHeader requestHeader = BuildRequestHeader(file.Header, headerId);

            _specialOrderDbContext.RequestHeaders.Add(requestHeader);

            _specialOrderDbContext.Context.SaveChanges();


            foreach (OrderDetail detail in file.Details) {
                _specialOrderDbContext.RequestItems.Add(BuildRequestDetail(detail, headerId, file.Header.Branch, file.Header.CustomerNumber, file.Header.PONumber));
            }

            _specialOrderDbContext.Context.SaveChanges();

            // add idToUse to order history
            var orderHistory = _orderHistory.ReadByConfirmationNumber(file.Header.ControlNumber.ToString().PadLeft(CONFNUMBER_LENGTH, CONFNUMBER_PADDINGCHAR), "B").First();

            //details
            foreach (var orderItem in orderHistory.OrderDetails) {
                var detailItems = file.Details.Where(x => x.ItemNumber == orderItem.ItemNumber).ToList();
                foreach (var detailItem in detailItems) {
                    orderItem.ManufacturerId = detailItem.ManufacturerName;//todo
                    orderItem.SpecialOrderLineNumber = detailItem.LineNumber.ToString();
                    orderItem.SpecialOrderHeaderId = headerId;
                }
            }
            _unitOfWork.SaveChangesAndClearContext();

            requestHeader.OrderStatusId = KSOS_STATUS_SEND;
            _specialOrderDbContext.Context.SaveChanges();
        }

        #endregion
    }
}
