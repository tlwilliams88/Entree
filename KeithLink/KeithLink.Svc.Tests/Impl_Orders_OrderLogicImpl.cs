using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Orders_OrderLogicImpl
    {
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
        public void SendMockOrder()
        {
            StubOrderLogicImpl order = new StubOrderLogicImpl(new OrderSocketConnectionRepositoryImpl());

            //order.ParseFile("somefilename");
            order.SendToHost(GetStubOrder());
        }

        [TestMethod]
        public void SerializeOrder() {
            OrderFile order = GetStubOrder();


            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(order.GetType());
            System.IO.StringWriter xmlWriter = new System.IO.StringWriter();

            xml.Serialize(xmlWriter, order);

            string xmlOutput = xmlWriter.ToString();
            Assert.IsTrue(xmlOutput.Length > 0);
        }


        [TestMethod]
        public void DeserializeOrder() {
            OrderFile order = GetStubOrder();

            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(order.GetType());
            System.IO.StringWriter xmlWriter = new System.IO.StringWriter();

            xml.Serialize(xmlWriter, order);

            string xmlOutput = xmlWriter.ToString();


            System.IO.StringReader xmlReader = new System.IO.StringReader(xmlOutput);
            OrderFile deserializedOrder = (OrderFile)xml.Deserialize(xmlReader);

            Assert.IsTrue(deserializedOrder.Header.ToString().Length > 0 && deserializedOrder.Details.Count == 2);
        }
    }
}
