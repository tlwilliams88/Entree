using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Cache;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class OrderHistoryLogicImplTests {
        #region attributes
        private ConfirmationLogicImpl _confLogic;
        private ElasticSearchCatalogRepositoryImpl _catRepo;
        private EventLogRepositoryImpl _log;
        private IOrderHistoryDetailRepository _detailRepo;
        private IOrderHistoryHeaderRepsitory _headerRepo;
        private OrderHistoryLogicImpl _logic;
        private GenericQueueRepositoryImpl _queue;
		private NoCacheRepositoryImpl _priceCache;
        private PriceLogicImpl _priceLogic;
        private PriceRepositoryImpl _priceRepo;
        private ProductImageRepositoryImpl _imgRepo;
        private PurchaseOrderRepositoryImpl _poRepo;
        private SiteCatalogLogicImpl _catalogLogic;
        private SocketListenerRepositoryImpl _socket;
        private IUnitOfWork _unitOfWork;

        private const string TEST_FILE = "Assets\\hourlyupdate.txt";
        #endregion

        #region ctor
        public OrderHistoryLogicImplTests() {
            _log = new EventLogRepositoryImpl("KeithLink Unit Tests");

            _priceRepo = new PriceRepositoryImpl();
            _priceLogic = new PriceLogicImpl(_priceRepo, _priceCache);
            _imgRepo = new ProductImageRepositoryImpl();

            _catRepo = new ElasticSearchCatalogRepositoryImpl();
            //_catalogLogic = new SiteCatalogLogicImpl(_catRepo, _priceLogic, _imgRepo, 
            _unitOfWork = new KeithLink.Svc.Impl.Repository.EF.Operational.UnitOfWork();
            _headerRepo = new KeithLink.Svc.Impl.Repository.Orders.History.EF.OrderHistoyrHeaderRepositoryImpl(_unitOfWork);
            _detailRepo = new KeithLink.Svc.Impl.Repository.Orders.History.EF.OrderHistoryDetailRepositoryImpl(_unitOfWork);
            _poRepo = new PurchaseOrderRepositoryImpl();

            _socket = new SocketListenerRepositoryImpl();
            _queue = new GenericQueueRepositoryImpl();

            //_confLogic = new ConfirmationLogicImpl(_log, _socket, new KeithLink.Svc.Impl.Repository.Queue.GenericQueueRepositoryImpl());

            //_logic = new OrderHistoryLogicImpl(_log, _headerRepo, _detailRepo, _queue,  _unitOfWork, _confLogic, _socket, _poRepo);
        }
        #endregion

        #region methods
        //[TestMethod]
        //public void ParseOrderHistoryFile() {
        //    OrderHistoryFileReturn parsedFile = _logic.ParseMainframeFile(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

        //    Assert.IsTrue(parsedFile.Files.Count > 0);
        //}

        //[TestMethod]
        //public void SuccessfulReadOfOrdersForCustomer() {
        //    _logic.GetOrders(new Core.Models.SiteCatalog.UserSelectedContext() {
        //            BranchId = "FDF",
        //            CustomerId = "024418"
        //        }
        //    );
        //}

        //[TestMethod]
        //public void UnsuccesfulReadOfOrdersForCustomer() {
        //    _logic.GetOrders(new Core.Models.SiteCatalog.UserSelectedContext() {
        //            BranchId = "FAM",
        //            CustomerId = "410300"
        //        }
        //    );
        //}
        #endregion
    }
}
