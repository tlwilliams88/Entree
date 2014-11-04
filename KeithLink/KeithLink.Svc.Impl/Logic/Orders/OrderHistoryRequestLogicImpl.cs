using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.IO;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryRequestLogicImpl : IOrderHistoryRequestLogic {

        #region attributes
        private readonly IOrderHistoryRequestQueueRepository _queue;
        #endregion

        #region ctor
        public OrderHistoryRequestLogicImpl(IOrderHistoryRequestQueueRepository queueRepo) {
            _queue = queueRepo;
        }
        #endregion

        #region methods
        private OrderHistoryRequest Desiarlize(string rawXml){
            OrderHistoryRequest request = new OrderHistoryRequest();

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(request.GetType());
            System.IO.StringReader xmlData = new System.IO.StringReader(rawXml);

            return (OrderHistoryRequest)xs.Deserialize(xmlData);
        }

        public void RequestAllOrdersForCustomer(UserSelectedContext context) {
            _queue.PublishToQueue(
                    SerializeRequest(
                        new OrderHistoryRequest() {
                            BranchId = context.BranchId,
                            CustomerNumber = context.CustomerId
                        }
                    )
                );
        }

        public void RequestOrderForCustomer(UserSelectedContext context, string invoiceNumber) {
            _queue.PublishToQueue(
                    SerializeRequest(
                        new OrderHistoryRequest() {
                            BranchId = context.BranchId,
                            CustomerNumber = context.CustomerId,
                            InvoiceNumber = invoiceNumber
                        }
                    )
                );
        }

        public string SerializeRequest(OrderHistoryRequest request) {
            StringWriter xml = new StringWriter();
            XmlSerializer xs = new XmlSerializer(request.GetType());

            xs.Serialize(xml, request);

            return xml.ToString();
        }
        #endregion
    }
}
