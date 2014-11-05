using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Events.EventArgs;
using CommerceServer.Core.Runtime.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Messaging;
using CommerceServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class ConfirmationLogicImpl : IConfirmationLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private ISocketListenerRepository _socket;
        private IGenericQueueRepository genericeQueueRepository;
        private Task queueListenerTask;
        private bool _keepQueueListening = true;
        private int queueListenerSleepTimeMs = 2000;

        private IQueueRepository _confirmationQueue;
        #endregion

        #region constructor
        public ConfirmationLogicImpl(IEventLogRepository eventLogRepository, ISocketListenerRepository socketListenerRepository,
            IQueueRepository confirmationQueue, IGenericQueueRepository internalMessagingLogic)
        {
            _log = eventLogRepository;
            _socket = socketListenerRepository;
            _confirmationQueue = confirmationQueue;
            this.genericeQueueRepository = internalMessagingLogic;

            _socket.FileReceived            += SocketFileReceived;
            _socket.ClosedPort              += SocketPortClosed;
            _socket.OpeningPort             += SocketOpeningPort;
            _socket.WaitingConnection       += SocketWaitingConnection;
            _socket.BeginningFileReceipt    += SocketBeginningFileReceipt;
            _socket.ErrorEncountered        += SocketExceptionEncountered;

        }
        #endregion

        #region methods/functions

        /// <summary>
        /// Begin listening for new confirmations
        /// </summary>
        public void ListenForMainFrameCalls()
        {
            _socket.Listen(Configuration.MainframeConfirmationListeningPort);
        }

        public void ListenForQueueMessages()
        {
            this.queueListenerTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        public void Stop()
        {
            _keepQueueListening = false;
            if (queueListenerTask != null && queueListenerTask.Status == TaskStatus.Running)
                queueListenerTask.Wait();
        }

        private void ListenForQueueMessagesInTask()
        {
            while (_keepQueueListening)
            {
                try
                {
                    System.Threading.Thread.Sleep(queueListenerSleepTimeMs);

                    ConfirmationFile confirmation = GetFileFromQueue();
                    if (confirmation != null)
                    {
                        try
                        {
                            if (ProcessIncomingConfirmation(confirmation) == false)
                            {
                                // If it fails we need to put the message back in the queue
                                PublishToQueue(confirmation, ConfirmationQueueLocation.Default);
                            }
                        }
                        catch (Exception e)
                        {
                            //HandleConfirmationQueueProcessingerror(e);
                            PublishToQueue(confirmation, ConfirmationQueueLocation.Default);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.WriteErrorLog("Error in MoveConfirmationsToCommerceServer", ex);
                }
            }
        }
        /// <summary>
        /// Deserialize the confirmation
        /// </summary>
        /// <param name="rawConfirmation"></param>
        /// <returns></returns>
        private ConfirmationFile DeserializeConfirmation(string rawConfirmation)
        {
            ConfirmationFile confirmation = new ConfirmationFile();

            StringReader reader = new StringReader(rawConfirmation);
            XmlSerializer xs = new XmlSerializer(confirmation.GetType());

            return (ConfirmationFile) xs.Deserialize(reader);
        }

        /// <summary>
        /// Send serialized file to RabbitMQ, send object to commerce server
        /// </summary>
        /// <param name="file"></param>
        public void ProcessFileData(string[] file)
        {
            try {
                ConfirmationFile confirmation = ParseFile( file );
                PublishToQueue( confirmation, ConfirmationQueueLocation.Default );
            } catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Publish confirmation file to queue
        /// </summary>
        /// <param name="file"></param>
        /// <param name="location"></param>
        public void PublishToQueue( ConfirmationFile file, ConfirmationQueueLocation location ) {
            string serializedConfirmation = SerializeConfirmation( file );

            _confirmationQueue.PublishToQueue(serializedConfirmation);
        }

        /// <summary>
        /// Get the current top Confirmation from the queue
        /// </summary>
        /// <returns></returns>
        public ConfirmationFile GetFileFromQueue() {
            string fileFromQueue = _confirmationQueue.ConsumeFromQueue();
            if (fileFromQueue == null)
                return null; // a null return indicates no message on queue
            else if (String.IsNullOrEmpty(fileFromQueue))
                throw new ApplicationException("Empty file from Confirmation Queue");

            return DeserializeConfirmation( fileFromQueue );
        }

        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ConfirmationFile ParseFile(string[] data)
        {
            ConfirmationFile confirmation = new ConfirmationFile();

            confirmation.Header.Parse(data[0]);

            // Start loop at detail, skip header
            for (int i = 1; i <= data.Length - 1; i++)
            {
                if (data[i].Contains("END###") == false)
                {
                    ConfirmationDetail theDeets = new ConfirmationDetail();
                    theDeets.Parse(data[i]);

                    confirmation.Detail.Add(theDeets);
                }
            }

            return confirmation;
        }

        /// <summary>
        /// Serialize the confirmation
        /// </summary>
        /// <param name="confirmation"></param>
        /// <returns></returns>
        private string SerializeConfirmation(ConfirmationFile confirmation)
        {
            StringWriter writer = new StringWriter();
            XmlSerializer xs = new XmlSerializer(confirmation.GetType());

            xs.Serialize(writer, confirmation);

            return writer.ToString();
        }

        public bool ProcessIncomingConfirmation(ConfirmationFile confirmation)
        {
            try
            {
                if (String.IsNullOrEmpty(confirmation.Header.ConfirmationNumber))
                    throw new ApplicationException("Confirmation Number is Required");
                if (String.IsNullOrEmpty(confirmation.Header.InvoiceNumber))
                    throw new ApplicationException("Invoice number is required");
                if (confirmation.Header.ConfirmationStatus == null)
                    throw new ApplicationException("Confirmation Status is Required");

                var poNum = confirmation.Header.ConfirmationNumber;
                PurchaseOrder po = GetCsPurchaseOrderByNumber(poNum);
                _log.WriteInformationLog("Processing confirmation for control number: " + confirmation.Header.ConfirmationNumber + ", did " + (po == null ? " not " : "") + "get purchase order");

                if (po == null)
                {
                    // if no PO, silently ignore?  could be the case if multiple control numbers out at once...
                }
                else
                {
                    // need to save away pre and post status info, then if different, add something to the messaging
                    LineItem[] currLineItems = new LineItem[po.LineItemCount];
                    LineItem[] origLineItems = new LineItem[po.LineItemCount];
                    po.OrderForms[0].LineItems.CopyTo(currLineItems, 0);
                    po.OrderForms[0].LineItems.CopyTo(origLineItems, 0);
                    string originalStatus = po.Status;

                    SetCsLineInfo(currLineItems, confirmation);

                    SetCsHeaderInfo(confirmation, po, currLineItems);

                    po.Save();

                    // use internal messaging logic to put order up message on the queue
                    Core.Models.Messaging.Queue.OrderChange orderChange = BuildOrderChanges(po, currLineItems, origLineItems, originalStatus);
                    if (orderChange.OriginalStatus != orderChange.CurrentStatus || orderChange.ItemChanges.Count > 0)
                    {
                        Core.Models.Messaging.Queue.OrderConfirmationNotification orderConfNotification = new Core.Models.Messaging.Queue.OrderConfirmationNotification();
                        orderConfNotification.OrderChange = orderChange;
                        orderConfNotification.CustomerNumber = (string)po["CustomerId"];
                        genericeQueueRepository.PublishToQueue(orderConfNotification.ToJson(), Configuration.RabbitMQNotificationServer, 
                            Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
                            Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error processing confirmation in internal service", ex);
                return false;
            }
            return true;
        }

        private Core.Models.Messaging.Queue.OrderChange BuildOrderChanges(PurchaseOrder po, LineItem[] currLineItems, LineItem[] origLineItems, string originalStatus)
        {
            Core.Models.Messaging.Queue.OrderChange orderChange = new Core.Models.Messaging.Queue.OrderChange();
            orderChange.OrderName = originalStatus;
            orderChange.CurrentStatus = po.Status;
            orderChange.ItemChanges = new List<Core.Models.Messaging.Queue.OrderLineChange>();
            orderChange.Items = new List<Core.Models.Messaging.Queue.OrderLineChange>();

            foreach (LineItem origItem in origLineItems)
            {
                LineItem newItem = currLineItems.Where(i => i.ProductId == origItem.ProductId).FirstOrDefault();
                if (newItem != null)
                {
                    if (origItem["MainFrameStatus"] != newItem["MainFrameStatus"])
                        orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = (string)newItem["MainFrameStatus"], OriginalStatus = (string)origItem["MainFrameStatus"], OriginalItemNumber = origItem.ProductId, SubstitutedItemNumber = (string)origItem["SubstituteItemNumber"], QuantityOrdered = (int)newItem["QuantityOrdered"], QuantityShipped = (int)newItem["QuantityShipped"] });
                }
                else
                {
                    orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = "Removed", OriginalStatus = "", OriginalItemNumber = origItem.ProductId, SubstitutedItemNumber = (string)origItem["SubstituteItemNumber"], QuantityOrdered = (int)origItem["QuantityOrdered"], QuantityShipped = (int)origItem["QuantityShipped"] }); // would we ever hit this?
                }
            }
            foreach (LineItem newItem in currLineItems)
            {
                LineItem origItem = origLineItems.Where(o => o.ProductId == newItem.ProductId).FirstOrDefault();
                if (origItem == null)
                    orderChange.ItemChanges.Add(new Core.Models.Messaging.Queue.OrderLineChange() { NewStatus = "Added", OriginalStatus = "", OriginalItemNumber = newItem.ProductId, SubstitutedItemNumber = (string)newItem["SubstituteItemNumber"], QuantityOrdered = (int)newItem["QuantityOrdered"], QuantityShipped = (int)newItem["QuantityShipped"] }); // would we ever hit this?
                orderChange.Items.Add(new Core.Models.Messaging.Queue.OrderLineChange() { OriginalItemNumber = (string)newItem.ProductId, SubstitutedItemNumber = (string)newItem["SubstituteItemNumber"], QuantityOrdered = (int)newItem["QuantityOrdered"], QuantityShipped = (int)newItem["QuantityShipped"] });
            }
            return orderChange;
        }

        private static PurchaseOrder GetCsPurchaseOrderByNumber(string poNum)
        {
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

            if (results.Tables.Count > 0 && results.Tables[0].Rows.Count > 0)
            {
                // Enumerate the results of the search.
                Guid soldToId = Guid.Parse(results.Tables[0].Rows[0].ItemArray[2].ToString());

                // get the guids for the customers associated users and loop if necessary
                PurchaseOrder po = Svc.Impl.Helpers.CommerceServerCore.GetOrderContext().GetPurchaseOrder(soldToId, poNum);
                return po;
            }
            else
            {
                return null;
            }
        }

        private void SetCsLineInfo(LineItem[] lineItems, ConfirmationFile confirmation)
        {
            foreach (var detail in confirmation.Detail)
            {
                // match incoming line items to CS line items
                int linePosition = Convert.ToInt32(detail.RecordNumber);

                LineItem orderFormLineItem = lineItems.Where(x => (int)x["LinePosition"] == (linePosition)).FirstOrDefault();

                if (orderFormLineItem != null)
                {
                    SetCsLineItemInfo(orderFormLineItem, detail.QuantityOrdered, detail.QuantityShipped, detail.DisplayStatus(), detail.SubstitueItemNumber());
                    _log.WriteInformationLog("Set main frame status: " + (string)orderFormLineItem["MainFrameStatus"] + ", confirmation status: _" + detail.DisplayStatus() + "_");
                }
                else
                    _log.WriteWarningLog("No CS line found for MainFrame line " + linePosition + " on order: " + confirmation.Header.InvoiceNumber);
            }
        }

        private void SetCsLineItemInfo(LineItem orderFormLineItem, int quantityOrdered, int quantityShipped, string displayStatus, string substituteItemNumber)
        {
            orderFormLineItem["QuantityOrdered"] = quantityOrdered;
            orderFormLineItem["QuantityShipped"] = quantityShipped;
            orderFormLineItem["MainFrameStatus"] = displayStatus;
            orderFormLineItem["SubstitueItemNumber"] = substituteItemNumber;
        }

        private string SetCsHeaderInfo(ConfirmationFile confirmation, PurchaseOrder po, LineItem[] lineItems)
        {
            string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
            if (trimmedConfirmationStatus == Constants.CONFIRMATION_HEADER_CONFIRMED_CODE)
            { // if confirmation status is blank, then look for exceptions across all line items, not just those in the change order
                string origOrderNumber = (string)po[Constants.CS_PURCHASE_ORDER_ORIGINAL_ORDER_NUMBER];
                string currOrderNumber = po.TrackingNumber;
                bool isChangeOrder = origOrderNumber != currOrderNumber;
                SetCsPoStatusFromLineItems(po, lineItems, isChangeOrder);
            }
            else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_IN_PROCESS_CODE))
            {
                po.Status = Constants.CONFIRMATION_HEADER_IN_PROCESS_STATUS;
            }
            else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_INVOICED_CODE))
            {
                po.Status = Constants.CONFIRMATION_HEADER_INVOICED_STATUS;
            }
            else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_DELETED_CODE))
            {
                po.Status = Constants.CONFIRMATION_HEADER_DELETED_STATUS;
            }
            else if (trimmedConfirmationStatus.Equals(Constants.CONFIRMATION_HEADER_REJECTED_CODE))
            {
                po.Status = Constants.CONFIRMATION_HEADER_REJECTED_STATUS;
            }

            po[Constants.CS_PURCHASE_ORDER_MASTER_NUMBER] = confirmation.Header.InvoiceNumber; // read this from the confirmation file

            _log.WriteInformationLog("Updating purchase order status with: " + po.Status + ", for confirmation status: _" + trimmedConfirmationStatus + "_");

            return trimmedConfirmationStatus;
        }

        private static void SetCsPoStatusFromLineItems(PurchaseOrder po, LineItem[] lineItems, bool isChangeOrder)
        {
            if (lineItems.Any(x => ((string)x[Constants.CS_LINE_ITEM_MAIN_FRAME_STATUS]) != Constants.CONFIRMATION_DETAIL_FILLED_STATUS))
            { // exceptions
                if (isChangeOrder)
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_EXCEPTIONS_STATUS;
                else
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_EXCEPTIONS_STATUS;
            }
            else
            { // no exceptions
                if (isChangeOrder)
                {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_STATUS;
                }
                else
                {
                    po.Status = Constants.CONFIRMATION_HEADER_CONFIRMED_STATUS;
                }
            }
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

        public void SocketExceptionEncountered(object sender, ExceptionEventArgs e)
        {
            _log.WriteErrorLog(e.Exception.Message);
        }

        #endregion

        #region properties
        #endregion
    }
}
