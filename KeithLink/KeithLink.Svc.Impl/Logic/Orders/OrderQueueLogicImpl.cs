using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders;
using CS = KeithLink.Svc.Core.Models.Generated;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class OrderQueueLogicImpl : IOrderQueueLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private IOrderSocketConnectionRepository _mfConnection;
        private IGenericQueueRepository _orderQueue;
        #endregion

        #region ctor
		public OrderQueueLogicImpl(IEventLogRepository eventLog, IGenericQueueRepository orderQueue, IOrderSocketConnectionRepository mfCon)
        {
            _log = eventLog;
            _mfConnection = mfCon;
            _orderQueue = orderQueue;

            AllowOrderProcessing = true;
        }
        #endregion

        #region methods
        
        public void ProcessOrders() {
            WorkOrderQueue(OrderQueueLocation.Normal);
            WorkOrderQueue(OrderQueueLocation.Reprocess);            
        }

        private void SendDetailRecordsToHost(List<OrderDetail> details, int ConfirmationNumber) {
            foreach (OrderDetail detail in details) {
                _mfConnection.Send(detail.ToMainframeFormat());

                bool waiting = true;

                do {
                    string detailReturnCode = _mfConnection.Receive();

                    if (detailReturnCode.Length == 0) {
                        throw new InvalidResponseException();
                    } else {
                        switch (detailReturnCode) {
                            case Constants.MAINFRAME_RECEIVE_STATUS_CANCELLED:
                                throw new CancelledTransactionException(ConfirmationNumber);
                            case Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN:
                                waiting = false;
                                break;
                            case Constants.MAINFRAME_RECEIVE_STATUS_WAITING:
                                waiting = true;
                                break;
                            default:
                                throw new InvalidResponseException();
                        }
                    }
                } while (waiting);
            }
        }

        private void SendEndOfRecordToHost() {
            // send end of records line
            _mfConnection.Send("*EOR");

            string eorReturnCode = _mfConnection.Receive();

            if (eorReturnCode.Length > 0 && eorReturnCode == Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN) {
            } else {
                throw new InvalidResponseException();
            }
        }

        private void SendHeaderRecordToHost(OrderHeader header) {
            _mfConnection.Send(header.ToMainframeFormat());

            // wait for a response from the mainframe
            bool waiting = true;
            do {
                string headerReturnCode = _mfConnection.Receive();

                if (headerReturnCode.Length == 0) {
                    throw new InvalidResponseException();
                } else {
                    switch (headerReturnCode) {
                        case Constants.MAINFRAME_RECEIVE_STATUS_CANCELLED:
                            throw new CancelledTransactionException(header.ControlNumber);
                        case Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN:
                            waiting = false;
                            break;
                        case Constants.MAINFRAME_RECEIVE_STATUS_WAITING:
                            waiting = true;
                            break;
                        default:
                            throw new CancelledTransactionException(header.ControlNumber);
                    }
                }
            } while (waiting);
        }

        private void SendStartTransaction(string ConfirmationNumber) {
            _mfConnection.StartTransaction(ConfirmationNumber.PadLeft(7, '0'));

            string startCode = _mfConnection.Receive();
            if (startCode.Length > 0 && startCode == Constants.MAINFRAME_RECEIVE_STATUS_GO) {
            } else {
                throw new ApplicationException("Invalid response received while starting the CICS transaction");
            }
        }

		private string GetSelectedExchange(OrderQueueLocation exchangeLocation) {
			switch (exchangeLocation)
			{
				case OrderQueueLocation.Normal:
					return Configuration.RabbitMQExchangeOrdersCreated;
				case OrderQueueLocation.History:
					return Configuration.RabbitMQExchangeOrdersHistory;
				case OrderQueueLocation.Error:
					return Configuration.RabbitMQExchangeOrdersError;
				default:
					return Configuration.RabbitMQExchangeOrdersCreated;
			}
		}

		private string GetSelectedQueue(OrderQueueLocation queueLocation)
		{
			switch (queueLocation)
			{
				case OrderQueueLocation.Normal:
					return Configuration.RabbitMQQueueOrderCreated;
				case OrderQueueLocation.History:
					return Configuration.RabbitMQQueueOrderHistory;
				case OrderQueueLocation.Error:
					return Configuration.RabbitMQQueueOrderError;
				default:
					return Configuration.RabbitMQQueueOrderCreated;
			}
		}

        private void SendToError(Exception ex, string errorOrder) {
			_orderQueue.PublishToQueue(errorOrder, Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostOrder, GetSelectedExchange(OrderQueueLocation.Error));
			KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, string.Format("Original order message: \r\n\r\n {0}", errorOrder), "An order has been placed on the Order Error Queue");
        }

        private void SendToHistory(string historyOrder)
        {
			_orderQueue.PublishToQueue(historyOrder, Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostOrder, GetSelectedExchange(OrderQueueLocation.History));
        }

        private void SendToReprocess(string errorOrder) {
			_orderQueue.PublishToQueue(errorOrder, Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostOrder, GetSelectedExchange(OrderQueueLocation.Reprocess));
        }

        private void SendToHost(OrderFile order)
        {
            // open connection and call program
            _mfConnection.Connect();

            SendStartTransaction(order.Header.ControlNumber.ToString());

            // start the order transmission to the mainframe
            _mfConnection.Send("OTX");

            SendHeaderRecordToHost(order.Header);
            SendDetailRecordsToHost(order.Details, order.Header.ControlNumber);
            SendEndOfRecordToHost();
            
            // stop order transmission to the mainframe
            _mfConnection.Send("STOP");

            _mfConnection.Close();
        }
		
        private void WorkOrderQueue(OrderQueueLocation queue) {

            string rawOrder = _orderQueue.ConsumeFromQueue(Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer,Configuration.RabbitMQVHostOrder, GetSelectedQueue(queue));

            while (!string.IsNullOrEmpty(rawOrder)) {
				
				OrderFile order = JsonConvert.DeserializeObject<OrderFile>(rawOrder);

                try {
                    _log.WriteInformationLog(string.Format("Sending order to mainframe({0})", order.Header.ControlNumber));

                    SendToHost(order);
                    SendToHistory(rawOrder);

                    _log.WriteInformationLog(string.Format("Order sent to mainframe({0})", order.Header.ControlNumber));
                } catch (Exception ex) {
                    _log.WriteErrorLog(string.Format("Error while sending order({0})", order.Header.ControlNumber), ex);

                    if (ex is EarlySocketException || ex is CancelledTransactionException) {
                        SendToReprocess(rawOrder);
                    } else {
                        SendToError(ex, rawOrder);
                    }

                    throw;
                }

				rawOrder = _orderQueue.ConsumeFromQueue(Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostOrder, GetSelectedQueue(queue));

            } 
        }

        public void WriteFileToQueue(string orderingUserEmail, string orderNumber, CS.PurchaseOrder newPurchaseOrder, OrderType orderType) {
            var newOrderFile = new OrderFile() {
                SenderApplicationName = Configuration.ApplicationName,
                SenderProcessName = "Send order to queue",

                Header = new OrderHeader() {
                    OrderingSystem = OrderSource.Entree,
                    Branch = newPurchaseOrder.Properties["BranchId"].ToString().ToUpper(),
                    CustomerNumber = newPurchaseOrder.Properties["CustomerId"].ToString(),
                    DeliveryDate = newPurchaseOrder.Properties["RequestedShipDate"].ToString().ToDateTime().Value,
                    PONumber = newPurchaseOrder.Properties["PONumber"] == null ? string.Empty : newPurchaseOrder.Properties["PONumber"].ToString(),
                    Specialinstructions = string.Empty,
                    ControlNumber = int.Parse(orderNumber),
                    OrderType = orderType,
                    InvoiceNumber = orderType == OrderType.NormalOrder ? string.Empty : (string)newPurchaseOrder.Properties["MasterNumber"],
                    OrderCreateDateTime = newPurchaseOrder.Properties["DateCreated"].ToString().ToDateTime().Value,
                    OrderSendDateTime = DateTime.Now,
                    UserId = orderingUserEmail.ToUpper(),
                    OrderFilled = false,
                    FutureOrder = false
                },
                Details = new List<OrderDetail>()
            };

            foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)newPurchaseOrder.Properties["LineItems"])) {
                var item = (CS.LineItem)lineItem.Target;
                if ((orderType == OrderType.ChangeOrder && String.IsNullOrEmpty(item.Status))
                    || orderType == OrderType.DeleteOrder) // do not include line items a) during a change order with no change or b) during a delete order
                    continue;

                OrderDetail detail = new OrderDetail() {
                    ItemNumber = item.ProductId,
                    OrderedQuantity = (short)item.Quantity,
                    UnitOfMeasure = ((bool)item.Each ? UnitOfMeasure.Package : UnitOfMeasure.Case),
                    SellPrice = (double)item.PlacedPrice,
                    Catchweight = (bool)item.CatchWeight,
                    LineNumber = Convert.ToInt16(lineItem.Target.Properties["LinePosition"]),
                    SubOriginalItemNumber = string.Empty,
                    ReplacedOriginalItemNumber = string.Empty
                };

                if (orderType == OrderType.ChangeOrder) {
                    switch (item.Status) {
                        case "added":
                            detail.ItemChange = LineType.Add;
                            break;
                        case "changed":
                            detail.ItemChange = LineType.Change;
                            break;
                        case "deleted":
                            detail.ItemChange = LineType.Delete;
                            break;
                        default:
                            detail.ItemChange = LineType.NoChange;
                            break;
                    }
                }

                newOrderFile.Details.Add(detail);
            }

			_log.WriteInformationLog(string.Format("Writing order to queue: {0}", JsonConvert.SerializeObject(newOrderFile)));

			_orderQueue.PublishToQueue(JsonConvert.SerializeObject(newOrderFile), Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostOrder, GetSelectedExchange(OrderQueueLocation.Normal));
      
        }
        #endregion

        #region properties
        public bool AllowOrderProcessing { get; set; }
        #endregion


    }
}
