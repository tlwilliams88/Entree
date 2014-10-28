using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders;
using CS = KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class OrderQueueLogicImpl : IOrderQueueLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private ISocketConnectionRepository _mfConnection;
        private IOrderQueueRepository _orderQueue;
        #endregion

        #region ctor
        public OrderQueueLogicImpl(IEventLogRepository eventLog, IOrderQueueRepository orderQueue, ISocketConnectionRepository mfCon)
        {
            _log = eventLog;
            _mfConnection = mfCon;
            _orderQueue = orderQueue;

            AllowOrderProcessing = true;
        }
        #endregion

        #region methods
        public OrderFile DeserializeOrder(string rawOrder) {
            OrderFile order = new OrderFile();

            StringReader xmlReader = new StringReader(rawOrder);
            XmlSerializer xs = new XmlSerializer(order.GetType());

            return (OrderFile)xs.Deserialize(xmlReader);
        }

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

        private void SendToError(string errorOrder) {
            _orderQueue.SetQueuePath((int)OrderQueueLocation.Error);

            _orderQueue.PublishToQueue(errorOrder);
        }

        private void SendToHistory(string historyOrder)
        {
            _orderQueue.SetQueuePath((int)OrderQueueLocation.History);

            _orderQueue.PublishToQueue(historyOrder);
        }

        private void SendToReprocess(string errorOrder) {
            _orderQueue.SetQueuePath((int)OrderQueueLocation.Reprocess);

            _orderQueue.PublishToQueue(errorOrder);
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

        private string SerializeOrder(OrderFile order) {
            StringWriter xmlWriter = new StringWriter();
            XmlSerializer xs = new XmlSerializer(order.GetType());

            xs.Serialize(xmlWriter, order);

            return xmlWriter.ToString();
        }

        private void WorkOrderQueue(OrderQueueLocation queue) {
            _orderQueue.SetQueuePath((int)queue);
            string rawOrder = _orderQueue.ConsumeFromQueue();

            while (rawOrder != null) {
                OrderFile order = DeserializeOrder(rawOrder);

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
                        SendToError(rawOrder);
                    }

                    throw;
                }

                _orderQueue.SetQueuePath((int)queue);
                rawOrder = _orderQueue.ConsumeFromQueue();
            } 
        }
        #endregion

        #region properties
        public bool AllowOrderProcessing { get; set; }
        #endregion


        public void WriteFileToQueue(string orderingUserEmail, string orderNumber, CS.PurchaseOrder newPurchaseOrder, OrderType orderType)
        {
            var newOrderFile = new OrderFile()
            {
                Header = new OrderHeader()
                {
                    OrderingSystem = OrderSource.Entree,
                    Branch = newPurchaseOrder.Properties["BranchId"].ToString().ToUpper(),
                    CustomerNumber = newPurchaseOrder.Properties["CustomerId"].ToString(),
                    DeliveryDate = newPurchaseOrder.Properties["RequestedShipDate"].ToString().ToDateTime().Value,
                    PONumber = string.Empty,
                    Specialinstructions = string.Empty,
                    ControlNumber = int.Parse(orderNumber),
                    OrderType = orderType,
                    InvoiceNumber = string.Empty,
                    OrderCreateDateTime = newPurchaseOrder.Properties["DateCreated"].ToString().ToDateTime().Value,
                    OrderSendDateTime = DateTime.Now,
                    UserId = orderingUserEmail.ToUpper(),
                    OrderFilled = false,
                    FutureOrder = false
                },
                Details = new List<OrderDetail>()
            };

            foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)newPurchaseOrder.Properties["LineItems"]))
            {
                var item = (CS.LineItem)lineItem.Target;
                if ((orderType == OrderType.ChangeOrder && (item.Status == null || String.IsNullOrEmpty(item.Status)))
                    || orderType == OrderType.DeleteOrder) // do not include line items a) during a change order with no change or b) during a delete order
                    continue;

                newOrderFile.Details.Add(new OrderDetail()
                {
                    ItemNumber = item.ProductId,
                    OrderedQuantity = (short)item.Quantity,
                    UnitOfMeasure = ((bool)item.Each ? UnitOfMeasure.Package : UnitOfMeasure.Case),
                    SellPrice = (double)item.PlacedPrice,
                    Catchweight = (bool)item.CatchWeight,
                    LineNumber = Convert.ToInt16(lineItem.Target.Properties["LinePosition"]),
                    ItemChange = LineType.Add,
                    SubOriginalItemNumber = string.Empty,
                    ReplacedOriginalItemNumber = string.Empty,
                    ItemStatus = orderType == OrderType.NormalOrder ? string.Empty : item.Status == "added" ? "A" : item.Status == "changed" ? "C" : "D"
                });
            }

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(newOrderFile.GetType());

            xs.Serialize(sw, newOrderFile);

            _orderQueue.PublishToQueue(sw.ToString());
        }
    }
}
