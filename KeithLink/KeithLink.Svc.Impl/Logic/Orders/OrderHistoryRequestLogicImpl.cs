using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryRequestLogicImpl : IOrderHistoryRequestLogic {

        #region attributes
        private readonly IEventLogRepository _log;
        private readonly IGenericQueueRepository _queue;
        private readonly ISocketConnectionRepository _socket;
        #endregion

        #region ctor
		public OrderHistoryRequestLogicImpl(IEventLogRepository logRepo, IGenericQueueRepository queueRepo, ISocketConnectionRepository socket)
		{
            _log = logRepo;
            _queue = queueRepo;
            _socket = socket;
        }
        #endregion

        #region methods
        public void ProcessRequests() {

		    string rawRequest = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueOrderUpdateRequest);

            while (!string.IsNullOrEmpty(rawRequest)) {
                OrderHistoryRequest request = JsonConvert.DeserializeObject<OrderHistoryRequest>(rawRequest);

                _log.WriteInformationLog(string.Format("Consuming order history request from queue for message ({0}).", request.MessageId));

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

				rawRequest = _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueOrderUpdateRequest);
            }
        }

        public void RequestAllOrdersForCustomer(UserSelectedContext context) {
            OrderHistoryRequest request = new OrderHistoryRequest() {
                SenderApplicationName = Configuration.ApplicationName,
                SenderProcessName = "Publish Order History Request for customer to queue",

                BranchId = context.BranchId.ToUpper(),
                CustomerNumber = context.CustomerId
            };

			_queue.PublishToQueue(JsonConvert.SerializeObject(request), Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeOrderUpdateRequests);

            _log.WriteInformationLog(string.Format("Publishing order history request to queue for message ({0}).", request.MessageId));
            _log.WriteInformationLog(string.Format("Request for all orders sent to queue - Branch: {0}, CustomerNumber: {1}", context.BranchId, context.CustomerId));
        }

        public void RequestOrderForCustomer(UserSelectedContext context, string invoiceNumber) {
            OrderHistoryRequest request = new OrderHistoryRequest() {
                SenderApplicationName = Configuration.ApplicationName,
                SenderProcessName = "Publish Order History Request for specific invoice to queue",

                BranchId = context.BranchId.ToUpper(),
                CustomerNumber = context.CustomerId,
                InvoiceNumber = invoiceNumber
            };

			_queue.PublishToQueue(JsonConvert.SerializeObject(request), Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeOrderUpdateRequests);

            _log.WriteInformationLog(string.Format("Publishing order history request to queue for message ({0}).", request.MessageId));
            _log.WriteInformationLog(string.Format("Request for order sent to queue - Branch: {0}, CustomerNumber: {1}, InvoiceNumber: {2}", context.BranchId, context.CustomerId, invoiceNumber));
        }

        #endregion
    }
}
