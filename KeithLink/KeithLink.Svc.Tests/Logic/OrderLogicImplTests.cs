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
            order.Header.ControlNumber = 32;
            order.Header.CustomerNumber = "709333";
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
