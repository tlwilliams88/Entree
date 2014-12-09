using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Enumerations.Order;

namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class OrderLogicImplTests
    {
        private OrderFile GetStubOrder() {
            OrderFile order = new OrderFile();

            order.Header.Branch = "FDF";
            order.Header.ControlNumber = 3407;
            order.Header.CustomerNumber = "709333";
            order.Header.DeliveryDate = DateTime.Now.AddDays(1);
            order.Header.OrderCreateDateTime = DateTime.Now;
            order.Header.OrderType = OrderType.NormalOrder;
            order.Header.OrderingSystem = OrderSource.Entree;
            order.Header.OrderSendDateTime = DateTime.Now;
            order.Header.PONumber = string.Empty;
            order.Header.Specialinstructions = string.Empty;
            order.Header.InvoiceNumber = string.Empty;
            order.Header.UserId = "KeithLink.Svc.Tests";


            order.Details.Add(new OrderDetail() {
                LineNumber = 1,
                ItemNumber = "001003",
                UnitOfMeasure = UnitOfMeasure.Case,
                OrderedQuantity = 1,
                SellPrice = 1.50,
                Catchweight = false,
                ItemChange = LineType.Add,
                ReplacedOriginalItemNumber = string.Empty,
                SubOriginalItemNumber = string.Empty,
                ItemStatus = string.Empty
            });
            order.Details.Add(new OrderDetail() {
                LineNumber = 2,
                ItemNumber = "002100",
                UnitOfMeasure = UnitOfMeasure.Case,
                OrderedQuantity = 1,
                SellPrice = 2.37,
                Catchweight = false,
                ItemChange = LineType.Add,
                ReplacedOriginalItemNumber = string.Empty,
                SubOriginalItemNumber = string.Empty,
                ItemStatus = string.Empty
            });

            return order;
        }

        [TestMethod]
        public void SendMockOrder()
        {
            // create an order and place it on the queue
            OrderQueueRepositoryImpl queue = new OrderQueueRepositoryImpl();
            queue.PublishToQueue(SerializeOrder(GetStubOrder()));


            OrderQueueLogicImpl orderLogic = new OrderQueueLogicImpl(new EventLogRepositoryImpl(Configuration.ApplicationName),
                                               queue,
                                               new OrderSocketConnectionRepositoryImpl());

            orderLogic.ProcessOrders();
        }

        //[TestMethod]
        //public void CallReadOrders() {
        //    EventLogRepositoryImpl logRepo = new EventLogRepositoryImpl(Configuration.ApplicationName);
        //    PurchaseOrderRepositoryImpl poRepo = new PurchaseOrderRepositoryImpl();
            
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.ElasticSearchCatalogRepositoryImpl catRepo = new Impl.Repository.SiteCatalog.ElasticSearchCatalogRepositoryImpl();
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.ProductImageRepositoryImpl catImgRepo = new Impl.Repository.SiteCatalog.ProductImageRepositoryImpl();

        //    KeithLink.Svc.Impl.Repository.SiteCatalog.PriceRepositoryImpl priceRepo = new Impl.Repository.SiteCatalog.PriceRepositoryImpl();
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.NoCachePriceCacheRepositoryImpl priceCache = new Impl.Repository.SiteCatalog.NoCachePriceCacheRepositoryImpl();
        //    KeithLink.Svc.Impl.Logic.PriceLogicImpl priceLogic = new Impl.Logic.PriceLogicImpl(priceRepo, priceCache);

        //    KeithLink.Svc.Impl.Repository.SiteCatalog.DivisionRepositoryImpl branchRepo = new Impl.Repository.SiteCatalog.DivisionRepositoryImpl();
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.NoDivisionServiceRepositoryImpl branchSvcRepo = new Impl.Repository.SiteCatalog.NoDivisionServiceRepositoryImpl();
        //    KeithLink.Svc.Impl.Logic.DivisionLogicImpl branchLogic = new Impl.Logic.DivisionLogicImpl(branchRepo, branchSvcRepo);
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.CategoryImageRepository categoryImgRepo = new Impl.Repository.SiteCatalog.CategoryImageRepository(logRepo);
        //    KeithLink.Svc.Impl.Repository.SiteCatalog.NoCacheCatalogCacheRepositoryImpl catCache = new Impl.Repository.SiteCatalog.NoCacheCatalogCacheRepositoryImpl();

        //    NoOrderServiceRepositoryImpl ordSvcRepo = new NoOrderServiceRepositoryImpl();
        //    KeithLink.Svc.Impl.Repository.Lists.NoListServiceRepositoryImpl listSvcRepo = new Impl.Repository.Lists.NoListServiceRepositoryImpl();

        //    KeithLink.Svc.Impl.Logic.SiteCatalog.SiteCatalogLogicImpl catLogic = new KeithLink.Svc.Impl.Logic.SiteCatalog.SiteCatalogLogicImpl(catRepo, priceLogic, catImgRepo, listSvcRepo, branchRepo, categoryImgRepo, catCache, branchLogic, ordSvcRepo);

        //    OrderQueueLogicImpl orderQueueLogic = new OrderQueueLogicImpl(logRepo, new OrderQueueRepositoryImpl(), new OrderSocketConnectionRepositoryImpl());

        //    OrderLogicImpl logic = new OrderLogicImpl(poRepo, catLogic, ordSvcRepo, listSvcRepo, orderQueueLogic, priceLogic, logRepo);

        //    System.Collections.Generic.List<KeithLink.Svc.Core.Models.Orders.Order> orders = 
        //        logic.ReadOrders(new Core.Models.Profile.UserProfile() {
        //                            UserId = Guid.Parse("{021b7601-8de6-4fd7-b8b0-314dcc6ba83b}"),
        //                            EmailAddress = "qauser@qa.com"
        //                         }, 
        //                         new Core.Models.SiteCatalog.UserSelectedContext() { 
        //                            BranchId = "FAM",
        //                            CustomerId = "410300"
        //                         });

        //    Assert.IsTrue(orders.Count > 0);
        //}

        private string SerializeOrder(OrderFile order) {
            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(order.GetType());
            System.IO.StringWriter xmlWriter = new System.IO.StringWriter();

            xml.Serialize(xmlWriter, order);

            return xmlWriter.ToString();
        }

        private OrderFile DeserializeOrder(string rawOrder) {
            OrderFile order = new OrderFile();

            System.IO.StringReader xmlReader = new System.IO.StringReader(rawOrder);
            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(order.GetType());

            return (OrderFile)xml.Deserialize(xmlReader);
        }
    }
}
