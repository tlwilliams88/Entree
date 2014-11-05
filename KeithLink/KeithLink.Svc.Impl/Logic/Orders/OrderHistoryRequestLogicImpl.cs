using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryRequestLogicImpl : IOrderHistoryRequestLogic {

        #region attributes
        private readonly IEventLogRepository _log;
        private readonly IOrderHistoryRequestQueueRepository _queue;
        private readonly ISocketConnectionRepository _socket;
        #endregion

        #region ctor
        public OrderHistoryRequestLogicImpl(IEventLogRepository logRepo, IOrderHistoryRequestQueueRepository queueRepo, ISocketConnectionRepository socket) {
            _log = logRepo;
            _queue = queueRepo;
            _socket = socket;
        }
        #endregion

        #region methods
        private OrderHistoryRequest Desiarlize(string rawXml){
            OrderHistoryRequest request = new OrderHistoryRequest();

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(request.GetType());
            System.IO.StringReader xmlData = new System.IO.StringReader(rawXml);

            return (OrderHistoryRequest)xs.Deserialize(xmlData);
        }

        public void ProcessRequests() {
            string rawRequest = _queue.ConsumeFromQueue();

            while (rawRequest != null) {
                OrderHistoryRequest request = Desiarlize(rawRequest);

                if (request.InvoiceNumber == null) { request.InvoiceNumber = string.Empty; }

                StringBuilder transactionData = new StringBuilder();
                transactionData.Append(request.BranchId);
                transactionData.Append(request.CustomerNumber);
                transactionData.Append(request.InvoiceNumber.PadLeft(8));

                _log.WriteInformationLog(string.Format("Requesting history from mainframe - Branch: {0}, CustomerNumber: {1}, InvoiceNumber: {2}", request.BranchId, request.CustomerNumber, request.InvoiceNumber));

                _socket.Connect();
                _socket.StartTransaction(transactionData.ToString());
                _socket.Close();

                _log.WriteInformationLog(string.Format("Request sent to mainframe - Branch: {0}, CustomerNumber: {1}, InvoiceNumber: {2}", request.BranchId, request.CustomerNumber, request.InvoiceNumber));

                rawRequest = _queue.ConsumeFromQueue();
            }
        }

        public void RequestAllOrdersForCustomer(UserSelectedContext context) {
            _queue.PublishToQueue(
                    SerializeRequest(
                        new OrderHistoryRequest() {
                            BranchId = context.BranchId.ToUpper(),
                            CustomerNumber = context.CustomerId
                        }
                    )
                );
            _log.WriteInformationLog(string.Format("Request for all orders sent to queue - Branch: {0}, CustomerNumber: {1}", context.BranchId, context.CustomerId));
        }

        public void RequestOrderForCustomer(UserSelectedContext context, string invoiceNumber) {
            _queue.PublishToQueue(
                    SerializeRequest(
                        new OrderHistoryRequest() {
                            BranchId = context.BranchId.ToUpper(),
                            CustomerNumber = context.CustomerId,
                            InvoiceNumber = invoiceNumber
                        }
                    )
                );
            _log.WriteInformationLog(string.Format("Request for order sent to queue - Branch: {0}, CustomerNumber: {1}, InvoiceNumber: {2}", context.BranchId, context.CustomerId, invoiceNumber));
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
