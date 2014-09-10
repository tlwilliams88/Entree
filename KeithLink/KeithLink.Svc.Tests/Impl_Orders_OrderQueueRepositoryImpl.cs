using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test {
    [TestClass]
    public class Impl_Orders_OrderQueueRepositoryImpl {

        private OrderFile GetStubOrder() {
            OrderFile order = new OrderFile();

            order.Header.Branch = "FDF";
            order.Header.ControlNumber = 1;
            order.Header.CustomerNumber = "001001";
            order.Header.DeliveryDate = DateTime.Now.AddDays(1);
            order.Header.OrderCreateDateTime = DateTime.Now;
            order.Header.OrderType = OrderType.NormalOrder;
            order.Header.OrderingSystem = OrderSource.KeithCom;
            order.Header.OrderSendDateTime = DateTime.Now;
            order.Header.PONumber = string.Empty;
            order.Header.Specialinstructions = string.Empty;
            order.Header.InvoiceNumber = string.Empty;
            order.Header.UserId = "KeithLink.Svc.Tests";


            order.Details.Add(new OrderDetail() {
                LineNumber = 1,
                ItemNumber = "000001",
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
                ItemNumber = "000002",
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
        public void ReceiveOrderFromQueue() {
            EventLogRepositoryImpl log = new EventLogRepositoryImpl(Configuration.ApplicationName);
            OrderSocketConnectionRepositoryImpl mfCon = new OrderSocketConnectionRepositoryImpl();
            StubOrderLogicImpl orderLogic = new StubOrderLogicImpl(mfCon);

            OrderQueueRepositoryImpl queue = new OrderQueueRepositoryImpl(log);

            //queue.ConsumeOrders();
        }

        [TestMethod]
        public void SendOrderToQueue() {
            EventLogRepositoryImpl log = new EventLogRepositoryImpl(Configuration.ApplicationName);
            OrderSocketConnectionRepositoryImpl mfCon = new OrderSocketConnectionRepositoryImpl();
            StubOrderLogicImpl orderLogic = new StubOrderLogicImpl(mfCon);

            OrderQueueRepositoryImpl queue = new OrderQueueRepositoryImpl(log);

            //queue.PublishOrder(GetStubOrder());
        }
    }
}
