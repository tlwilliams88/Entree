using CommerceServer.Core;
using CommerceServer.Core.Runtime.Orders;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class ConfirmationLogicImpl : IConfirmationLogic
    {
        #region attributes
        private IGenericQueueRepository genericeQueueRepository;
        private IOrderConversionLogic _conversionLogic;
        private IEventLogRepository _log;
        private ISocketListenerRepository _socket;
        private readonly IUnitOfWork _unitOfWork;
        private Task queueListenerTask;
        private bool _keepQueueListening = true;
        private int queueListenerSleepTimeMs = 2000;
        #endregion

        #region constructor
        public ConfirmationLogicImpl(IEventLogRepository eventLogRepository, ISocketListenerRepository socketListenerRepository,
                                     IGenericQueueRepository internalMessagingLogic, IOrderConversionLogic conversionLogic, IUnitOfWork unitOfWork) {
            _log = eventLogRepository;
            _socket = socketListenerRepository;
            this.genericeQueueRepository = internalMessagingLogic;
            _conversionLogic = conversionLogic;
            _unitOfWork = unitOfWork;

            _socket.FileReceived            += SocketFileReceived;
            _socket.ClosedPort              += SocketPortClosed;
            _socket.OpeningPort             += SocketOpeningPort;
            _socket.WaitingConnection       += SocketWaitingConnection;
            _socket.BeginningFileReceipt    += SocketBeginningFileReceipt;
            _socket.ErrorEncountered        += SocketExceptionEncountered;

        }
        #endregion

        #region events

        public void SocketFileReceived(object sender, ReceivedFileEventArgs e)
        {
            string[] lines = e.FileData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            ProcessFileData(lines);
        }

        public void SocketPortClosed(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port closed");
        }

        public void SocketOpeningPort(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port opening");
        }

        public void SocketWaitingConnection(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener port connecting");
        }

        public void SocketBeginningFileReceipt(object sender, EventArgs e)
        {
            _log.WriteInformationLog("Confirmation listener beginning file receipt");
        }

        public void SocketExceptionEncountered(object sender, ExceptionEventArgs e) {
            _log.WriteErrorLog(string.Concat("Exception encountered in ConfirmationLogic: ", e.Exception.Message));
            _log.WriteWarningLog("Listener will stop processing and will need to be restarted");

            KeithLink.Common.Core.Email.ExceptionEmail.Send(e.Exception, "Listener will stop processing and will need to be restarted");
        }

        #endregion

        #region methods/functions
        private Core.Models.Messaging.Queue.OrderChange BuildOrderChanges(PurchaseOrder po, LineItem[] currLineItems, LineItem[] origLineItems, string originalStatus) {
            Core.Models.Messaging.Queue.OrderChange orderChange = new Core.Models.Messaging.Queue.OrderChange();
            orderChange.OrderName = (string)po["DisplayName"];
            orderChange.OriginalStatus = originalStatus;
            orderChange.CurrentStatus = po.Status;
            orderChange.ItemChanges = new List<Core.Models.Messaging.Queue.OrderLineChange>();
            orderChange.Items = new List<Core.Models.Messaging.Queue.OrderLineChange>();

            foreach (LineItem origItem in origLineItems) {
                LineItem newItem = currLineItems.Where(i => i.ProductId == origItem.ProductId).FirstOrDefault();
                if (newItem != null) {
                    if (origItem["MainFrameStatus"] != newItem["MainFrameStatus"])
                        orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = (string)newItem["MainFrameStatus"], OriginalStatus = (string)origItem["MainFrameStatus"], ItemNumber = origItem.ProductId, SubstitutedItemNumber = (string)origItem["SubstitutedItemNumber"], QuantityOrdered = (int)newItem["QuantityOrdered"], QuantityShipped = (int)newItem["QuantityShipped"] });
                } else {
                    orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = "Removed", OriginalStatus = "", ItemNumber = origItem.ProductId, SubstitutedItemNumber = (string)origItem["SubstitutedItemNumber"], QuantityOrdered = (int)origItem["QuantityOrdered"], QuantityShipped = (int)origItem["QuantityShipped"] }); // would we ever hit this?
                }
            }
            foreach (LineItem newItem in currLineItems) {
                LineItem origItem = origLineItems.Where(o => o.ProductId == newItem.ProductId).FirstOrDefault();
                if (origItem == null)
                    orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = "Added", OriginalStatus = "", ItemNumber = newItem.ProductId, SubstitutedItemNumber = (string)newItem["SubstitutedItemNumber"], QuantityOrdered = (int)newItem["QuantityOrdered"], QuantityShipped = (int)newItem["QuantityShipped"] }); // would we ever hit this?

                orderChange.Items.Add(new Core.Models.Messaging.Queue.OrderLineChange() {
                    ItemNumber = (string)newItem.ProductId,
                    ItemDescription = newItem.DisplayName,
                    SubstitutedItemNumber = newItem["SubstitutedItemNumber"] == null ? string.Empty : (string)newItem["SubstitutedItemNumber"],
                    QuantityOrdered = newItem["QuantityOrdered"] == null ? (int)newItem.Quantity : (int)newItem["QuantityOrdered"],
                    QuantityShipped = newItem["QuantityShipped"] == null ? 0 : (int)newItem["QuantityShipped"]
                });
            }
            return orderChange;
        }

        /// <summary>
        /// Deserialize the confirmation
        /// </summary>
        /// <param name="rawConfirmation"></param>
        /// <returns></returns>
       
        private static PurchaseOrder GetCsPurchaseOrderByNumber(string poNum) {
            System.Data.DataSet searchableProperties = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchableProperties(System.Globalization.CultureInfo.CurrentUICulture.ToString());
            SearchClauseFactory searchClauseFactory = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().GetSearchClauseFactory(searchableProperties, "PurchaseOrder");
            SearchClause trackingNumberClause = searchClauseFactory.CreateClause(ExplicitComparisonOperator.Equal, "TrackingNumber", poNum);

            // Create search options.

            SearchOptions options = new SearchOptions();
            options.PropertiesToReturn = "SoldToId";
            options.SortProperties = "SoldToId";
            options.NumberOfRecordsToReturn = 1;
            // Perform the search.
            System.Data.DataSet results = Svc.Impl.Helpers.CommerceServerCore.GetPoManager().SearchPurchaseOrders(trackingNumberClause, options);

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0) {
                // Enumerate the results of the search.
                Guid soldToId = Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());

                // get the guids for the customers associated users and loop if necessary
                PurchaseOrder po = Svc.Impl.Helpers.CommerceServerCore.GetOrderContext().GetPurchaseOrder(soldToId, poNum);
                return po;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Get the current top Confirmation from the queue
        /// </summary>
        /// <returns></returns>
        public ConfirmationFile GetFileFromQueue() {
            string fileFromQueue = genericeQueueRepository.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer,
                Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueConfirmation);
            if (fileFromQueue == null)
                return null; // a null return indicates no message on queue
            else if (String.IsNullOrEmpty(fileFromQueue))
                throw new ApplicationException("Empty file from Confirmation Queue");

            return JsonConvert.DeserializeObject<ConfirmationFile>(fileFromQueue);
        }

        /// <summary>
        /// Begin listening for new confirmations
        /// </summary>
        public void ListenForMainFrameCalls() {
            _socket.Listen(Configuration.MainframeConfirmationListeningPort);
        }

        public void ListenForQueueMessages() {
            this.queueListenerTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while (_keepQueueListening) {
                try {
                    System.Threading.Thread.Sleep(queueListenerSleepTimeMs);

                    ConfirmationFile confirmation = GetFileFromQueue();
                    if (confirmation != null) {
                        try {
                            _log.WriteInformationLog(string.Format("Pulling confirmation from queue using message ({0})", confirmation.MessageId));

                            ProcessIncomingConfirmation(confirmation);
                            _conversionLogic.SaveConfirmationAsOrderHistory(confirmation);
                        } catch (Exception e) {
                            _log.WriteErrorLog("Error processing confirmation in internal service", e);
                            KeithLink.Common.Core.Email.ExceptionEmail.Send(e);

                            confirmation.ErrorMessage = e.Message;
                            confirmation.ErrorStack = e.StackTrace;

                            PublishToQueue(confirmation, ConfirmationQueueLocation.Error);
                        }
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error Moving Confirmations To Commerce Server", ex);
                }
            }
        }

        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ConfirmationFile ParseFile(string[] data) {
            ConfirmationFile confirmation = new ConfirmationFile();

            confirmation.Header.Parse(data[0]);

            // Start loop at detail, skip header
            for (int i = 1; i <= data.Length - 1; i++) {
                if (data[i].Contains("END###") == false) {
                    ConfirmationDetail theDeets = new ConfirmationDetail();
                    theDeets.Parse(data[i]);

                    confirmation.Detail.Add(theDeets);
                }
            }

            return confirmation;
        }

        /// <summary>
        /// Send serialized file to RabbitMQ, send object to commerce server
        /// </summary>
        /// <param name="file"></param>
        public void ProcessFileData(string[] file) {
            try {
                ConfirmationFile confirmation = ParseFile(file);

                confirmation.SenderApplicationName = Configuration.ApplicationName;
                confirmation.SenderProcessName = "Receive Confirmation From Mainframe";

                PublishToQueue(confirmation, ConfirmationQueueLocation.Default);
            } catch (Exception e) {
                throw e;
            }
        }

        public bool ProcessIncomingConfirmation(ConfirmationFile confirmation) {
            if (String.IsNullOrEmpty(confirmation.Header.ConfirmationNumber))
                throw new ApplicationException("Confirmation Number is Required");
            if (String.IsNullOrEmpty(confirmation.Header.InvoiceNumber))
                throw new ApplicationException("Invoice number is required");
            if (confirmation.Header.ConfirmationStatus == null)
                throw new ApplicationException("Confirmation Status is Required");

            var poNum = confirmation.Header.ConfirmationNumber;
            PurchaseOrder po = GetCsPurchaseOrderByNumber(poNum);
            _log.WriteInformationLog("Processing confirmation for control number: " + confirmation.Header.ConfirmationNumber + ", did " + (po == null ? " not " : "") + "get purchase order");

            if (po == null) {
                // if no PO, silently ignore?  could be the case if multiple control numbers out at once...
            } else {
                // need to save away pre and post status info, then if different, add something to the messaging
                LineItem[] currLineItems = new LineItem[po.LineItemCount];
                LineItem[] origLineItems = new LineItem[po.LineItemCount];
                po.OrderForms[0].LineItems.CopyTo(currLineItems, 0);
                po.OrderForms[0].LineItems.CopyTo(origLineItems, 0);
                string originalStatus = po.Status;

                if (confirmation.Header.ConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_REJECTED_CODE, StringComparison.InvariantCultureIgnoreCase)) {
                    // do not update the line items if the order is rejected
                } else {
                    SetCsLineInfo(currLineItems, confirmation);
                }

                SetCsHeaderInfo(confirmation, po, currLineItems);

                po.Save();

                // use internal messaging logic to put order up message on the queue
                Core.Models.Messaging.Queue.OrderChange orderChange = BuildOrderChanges(po, currLineItems, origLineItems, originalStatus);
                if (orderChange.OriginalStatus != orderChange.CurrentStatus || orderChange.ItemChanges.Count > 0) {
                    Core.Models.Messaging.Queue.OrderConfirmationNotification orderConfNotification = new Core.Models.Messaging.Queue.OrderConfirmationNotification();
                    orderConfNotification.OrderChange = orderChange;
                    orderConfNotification.CustomerNumber = (string)po["CustomerId"];
                    genericeQueueRepository.PublishToQueue(orderConfNotification.ToJson(), Configuration.RabbitMQNotificationServer,
                        Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
                        Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
                }
            }

            return true;
        }

        /// <summary>
        /// Publish confirmation file to queue
        /// </summary>
        /// <param name="file"></param>
        /// <param name="location"></param>
        public void PublishToQueue(ConfirmationFile file, ConfirmationQueueLocation location) {
            string serializedConfirmation = JsonConvert.SerializeObject(file);

            _log.WriteInformationLog(string.Format("Writing confirmation to the queue for message ({0}).{1}{1}{2}", file.MessageId, "\r\n", serializedConfirmation));

            switch (location) {
                case ConfirmationQueueLocation.Default:
                    genericeQueueRepository.PublishToQueue(serializedConfirmation, Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNamePublisher,
                        Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeConfirmation);
                    break;
                case ConfirmationQueueLocation.Error:
                    genericeQueueRepository.PublishToQueue(serializedConfirmation, Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNamePublisher,
                        Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeConfirmationErrors);
                    break;
                default:
                    break;
            }
        }
		
		private string SetCsHeaderInfo(ConfirmationFile confirmation, PurchaseOrder po, LineItem[] lineItems) {
            string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
            switch (trimmedConfirmationStatus) {
                case Constants.CONFIRMATION_HEADER_CONFIRMED_CODE:
                    string origOrderNumber = (string)po[Constants.CS_PURCHASE_ORDER_ORIGINAL_ORDER_NUMBER];
                    string currOrderNumber = po.TrackingNumber;
                    bool isChangeOrder = origOrderNumber != currOrderNumber;
                    SetCsPoStatusFromLineItems(po, lineItems, isChangeOrder);
                    break;
                case Constants.CONFIRMATION_HEADER_IN_PROCESS_CODE:
                    po.Status = Constants.CONFIRMATION_HEADER_IN_PROCESS_STATUS;
                    break;
                case Constants.CONFIRMATION_HEADER_INVOICED_CODE:
                    po.Status = Constants.CONFIRMATION_HEADER_INVOICED_STATUS;
                    break;
                case Constants.CONFIRMATION_HEADER_DELETED_CODE:
                    po.Status = Constants.CONFIRMATION_HEADER_DELETED_STATUS;
                    break;
                case Constants.CONFIRMATION_HEADER_REJECTED_CODE:
                    po.Status = Constants.CONFIRMATION_HEADER_REJECTED_STATUS;
                    break;
            }

            if (confirmation.Header.ConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_REJECTED_CODE, StringComparison.InvariantCultureIgnoreCase)){
                po[Constants.CS_PURCHASE_ORDER_MASTER_NUMBER] = Constants.CONFIRMATION_HEADER_REJECTED_STATUS;
            } else {
                po[Constants.CS_PURCHASE_ORDER_MASTER_NUMBER] = confirmation.Header.InvoiceNumber; // read this from the confirmation file
            }

            _log.WriteInformationLog("Updating purchase order status with: " + po.Status + ", for confirmation status: _" + trimmedConfirmationStatus + "_");

            return trimmedConfirmationStatus;
        }

        private void SetCsLineInfo(LineItem[] lineItems, ConfirmationFile confirmation) {
            foreach (var detail in confirmation.Detail) {
                // match incoming line items to CS line items
                int linePosition = Convert.ToInt32(detail.RecordNumber);

                LineItem orderFormLineItem = lineItems.Where(x => (int)x["LinePosition"] == (linePosition)).FirstOrDefault();

                if (orderFormLineItem != null) {
                    SetCsLineItemInfo(orderFormLineItem, detail.QuantityOrdered, detail.QuantityShipped, detail.DisplayStatus(), detail.ItemNumber, detail.SubstitutedItemNumber(orderFormLineItem));
                    _log.WriteInformationLog("Set main frame status: " + (string)orderFormLineItem["MainFrameStatus"] + ", confirmation status: _" + detail.DisplayStatus() + "_");
                } else
                    _log.WriteWarningLog("No CS line found for MainFrame line " + linePosition + " on order: " + confirmation.Header.InvoiceNumber);
            }
        }

        private void SetCsLineItemInfo(LineItem orderFormLineItem, int quantityOrdered, int quantityShipped, string displayStatus, string currentItemNumber, string substitutedItemNumber) {
            orderFormLineItem["QuantityOrdered"] = quantityOrdered;
            orderFormLineItem["QuantityShipped"] = quantityShipped;
            orderFormLineItem["MainFrameStatus"] = displayStatus;
            orderFormLineItem["SubstitutedItemNumber"] = substitutedItemNumber;
            orderFormLineItem.ProductId = currentItemNumber;
        }

        private static void SetCsPoStatusFromLineItems(PurchaseOrder po, LineItem[] lineItems, bool isChangeOrder) {
            if (lineItems.Any(x => ((string)x[Constants.CS_LINE_ITEM_MAIN_FRAME_STATUS]) != Constants.CONFIRMATION_DETAIL_FILLED_STATUS)) { // exceptions
                if (isChangeOrder)
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_EXCEPTIONS_STATUS;
                else
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_EXCEPTIONS_STATUS;
            } else { // no exceptions
                if (isChangeOrder) {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_STATUS;
                } else {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_STATUS;
                }
            }
        }

        public void Stop() {
            _keepQueueListening = false;
            if (queueListenerTask != null && queueListenerTask.Status == TaskStatus.Running)
                queueListenerTask.Wait();
        }
        #endregion

        #region properties
        #endregion
    }
}
