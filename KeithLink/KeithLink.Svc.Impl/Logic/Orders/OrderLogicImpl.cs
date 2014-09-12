using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class OrderLogicImpl : IOrderLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private ISocketConnectionRepository _mfConnection;
        private IQueueRepository _orderQueue;
        #endregion

        #region ctor
        public OrderLogicImpl(IEventLogRepository eventLog, IQueueRepository orderQueue, ISocketConnectionRepository mfCon)
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
            _log.WriteInformationLog("Start monitoring the order queue");

            while (AllowOrderProcessing) {
                _orderQueue.SetQueuePath((int)OrderQueueLocation.Normal);

                string rawOrder = _orderQueue.ConsumeFromQueue();

                if (rawOrder == null) {
                    System.Threading.Thread.Sleep(2000);
                } else {
                    OrderFile order = DeserializeOrder(rawOrder);

                    try {
                        _log.WriteInformationLog(string.Format("Sending order to mainframe({0})", order.Header.ControlNumber));

                        SendToHost(order);
                        SendToHistory(rawOrder);

                        _log.WriteInformationLog(string.Format("Order sent to mainframe({0})", order.Header.ControlNumber));
                    } catch (Exception ex) {
                        _log.WriteErrorLog(string.Format("Error while sending order({0})", order.Header.ControlNumber), ex);

                        SendToError(rawOrder);
                    }
                }
            }

            _log.WriteInformationLog("No longer watching the order queue");
        }

        private void SendDetailRecordsToHost(List<OrderDetail> details) {
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
                                throw new CancelledTransactionException();
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
                            throw new CancelledTransactionException();
                        case Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN:
                            waiting = false;
                            break;
                        case Constants.MAINFRAME_RECEIVE_STATUS_WAITING:
                            waiting = true;
                            break;
                        default:
                            throw new CancelledTransactionException();
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

        private void SendToHost(OrderFile order)
        {
            // open connection and call program
            _mfConnection.Connect();

            SendStartTransaction(order.Header.ControlNumber.ToString());

            // start the order transmission to the mainframe
            _mfConnection.Send("OTX");

            SendHeaderRecordToHost(order.Header);
            SendDetailRecordsToHost(order.Details);
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
        #endregion

        #region properties
        public bool AllowOrderProcessing { get; set; }
        #endregion
    }
}
