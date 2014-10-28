using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Confirmations;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.Confirmations
{
    public class ConfirmationLogicImpl : IConfirmationLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private ISocketListenerRepository _socket;

        private IQueueRepository _confirmationQueue;
        #endregion

        #region constructor
        public ConfirmationLogicImpl(IEventLogRepository eventLogRepository, ISocketListenerRepository socketListenerRepository, IQueueRepository confirmationQueue)
        {
            _log = eventLogRepository;
            _socket = socketListenerRepository;
            _confirmationQueue = confirmationQueue;

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
        public void Listen()
        {
            _socket.Listen();
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

        List<CsOrderLineUpdateInfo> GetCsLineUpdateInfo(ConfirmationFile confirmationFile)
        {
            List<CsOrderLineUpdateInfo> ret = new List<CsOrderLineUpdateInfo>();
            foreach (var detail in confirmationFile.Detail)
            {
                CsOrderLineUpdateInfo line = new CsOrderLineUpdateInfo()
                {
                    MainFrameStatus = detail.ReasonNotShipped.Trim(),
                    SubstitueItemNumber = detail.ItemNumber,
                    QuantityOrdered = detail.QuantityOrdered,
                    QuantityShipped = detail.QuantityShipped,
                    RecordNumber = int.Parse(detail.RecordNumber)
                };
                ret.Add(line);
            }
            return ret;
        }

        private bool ProcessIncomingConfirmation(ConfirmationFile confirmation)
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
                    string trimmedConfirmationStatus = SetCsHeaderInfo(confirmation, po);

                    LineItem[] lineItems = new LineItem[po.LineItemCount];
                    po.OrderForms[0].LineItems.CopyTo(lineItems, 0);

                    SetCsLineInfo(trimmedConfirmationStatus, lineItems, GetCsLineUpdateInfo(confirmation));
                    po.Save();
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error processing confirmation in internal service", ex);
                return false;
            }
            return true;
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

        private void SetCsLineInfo(string trimmedConfirmationStatus, LineItem[] lineItems, List<CsOrderLineUpdateInfo> confirmationDetail)
        {
            foreach (var detail in confirmationDetail)
            {
                // match up to incoming line items to CS line items
                int index = detail.RecordNumber - 1;
                if (index >= lineItems.Length)
                    continue; // TODO: log this?  shouldn't happen, but who knows...

                LineItem orderFormLineItem = lineItems.Where(x => (int)x["LinePosition"] == (detail.RecordNumber)).FirstOrDefault();
                string confirmationStatus = detail.MainFrameStatus.Trim().ToUpper();

                orderFormLineItem["QuantityOrdered"] = detail.QuantityOrdered;
                orderFormLineItem["QuantityShipped"] = detail.QuantityShipped;
                _log.WriteInformationLog("Setting main frame status");
                if (String.IsNullOrEmpty(confirmationStatus))
                {
                    orderFormLineItem["MainFrameStatus"] = "Filled";
                }
                if (confirmationStatus == "P") // partial ship
                {
                    orderFormLineItem["MainFrameStatus"] = "Partially Shipped";
                }
                else if (confirmationStatus == "O") // out of stock
                {
                    orderFormLineItem["MainFrameStatus"] = "Out of Stock";
                }
                else if (confirmationStatus == "R") // item replaced
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Replaced";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "Z") // item replaced, but replacement currently out of stock
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Replaced, Out of Stock";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "T") // Item replaced, partial fill
                {
                    orderFormLineItem["MainFrameStatus"] = "Partially Shipped, Item Replaced";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                else if (confirmationStatus == "S") // item subbed
                {
                    orderFormLineItem["MainFrameStatus"] = "Item Subbed";
                    orderFormLineItem["SubstitueItemNumber"] = detail.SubstitueItemNumber;
                }
                _log.WriteInformationLog("Set main frame status: " + (string)orderFormLineItem["MainFrameStatus"] + ", confirmation status: _" + confirmationStatus + "_");
            }
        }

        private string SetCsHeaderInfo(ConfirmationFile confirmation, PurchaseOrder po)
        {
            // get header status into CS
            // values are " ", "P", "I", "D" = " " open, "P" Processing, "I" Invoicing, "D" Delete
            string trimmedConfirmationStatus = confirmation.Header.ConfirmationStatus.Trim().ToUpper();
            if (String.IsNullOrEmpty(trimmedConfirmationStatus))
            {
                po.Status = "NewOrder";
            }
            else if (trimmedConfirmationStatus.Equals("P"))
            {
                po.Status = "Submitted";
            }
            else if (trimmedConfirmationStatus.Equals("I"))
            {
                po.Status = "InProcess";
            }
            else if (trimmedConfirmationStatus.Equals("D"))
            {
                po.Status = "Cancelled";
            }


            po["MasterNumber"] = confirmation.Header.InvoiceNumber; // read this from the confirmation file

            _log.WriteInformationLog("Updating purchase order status with: " + po.Status + ", for confirmation status: _" + trimmedConfirmationStatus + "_");

            return trimmedConfirmationStatus;
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
