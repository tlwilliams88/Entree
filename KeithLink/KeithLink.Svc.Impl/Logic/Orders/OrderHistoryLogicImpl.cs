using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Enumerations.Order;

using KeithLink.Svc.Core.Events.EventArgs;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;

using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryLogicImpl : IOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;
        private const int THREAD_SLEEP_DURATION = 2000;

        private readonly ICatalogLogic _catalogLogic;
        private readonly IOrderConversionLogic _conversionLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IKPayInvoiceRepository _kpayInvoiceRepository;
        private readonly IEventLogRepository _log;
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IGenericQueueRepository _queue;
        private readonly ISocketListenerRepository _socket;
        private readonly IUnitOfWork _unitOfWork;

        private bool _keepListening;
        private Task _queueTask;
        #endregion

        #region ctor
        public OrderHistoryLogicImpl(IOrderHistoryHeaderRepsitory headerRepo, IPurchaseOrderRepository poRepo, IKPayInvoiceRepository kpayInvoiceRepository, 
                                     ICatalogLogic catalogLogic, IOrderHistoryDetailRepository detailRepo, IUnitOfWork unitOfWork, 
                                     IEventLogRepository log, IGenericQueueRepository queue, IOrderConversionLogic conversionLogic, 
                                     ICustomerRepository customerRepository, ISocketListenerRepository socket) {
            _log = log;
            _queue = queue;
            _socket = socket;

            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
            _poRepo = poRepo;
            _kpayInvoiceRepository = kpayInvoiceRepository;
            _catalogLogic = catalogLogic;
            _unitOfWork = unitOfWork;
            _conversionLogic = conversionLogic;
            _keepListening = true;
            _customerRepository = customerRepository;

            _socket.FileReceived            += SocketFileReceived;
            _socket.ClosedPort              += SocketPortClosed;
            _socket.OpeningPort             += SocketOpeningPort;
            _socket.WaitingConnection       += SocketWaitingConnection;
            _socket.BeginningFileReceipt    += SocketBeginningFileReceipt;
            _socket.ErrorEncountered        += SocketExceptionEncountered;
        }
        #endregion

        #region events
        public void SocketFileReceived(object sender, ReceivedFileEventArgs e) {
            StringBuilder logMsg = new StringBuilder();
            logMsg.AppendLine("Order Update File Received. See below for more details.");
            logMsg.AppendLine();
            logMsg.AppendLine(e.FileData);

            _log.WriteInformationLog(logMsg.ToString());
            string[] lines = e.FileData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            OrderHistoryFileReturn parsedFiles = ParseFile(lines);

            foreach (OrderHistoryFile parsedFile in parsedFiles.Files) {
                parsedFile.SenderApplicationName = Configuration.ApplicationName;
                parsedFile.SenderProcessName = "Process Order History Updates From Mainframe (Socket Connection)";

				var jsonValue = JsonConvert.SerializeObject(parsedFile);

				_queue.PublishToQueue(jsonValue, Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeHourlyUpdates);

                logMsg = new StringBuilder();
                logMsg.AppendLine(string.Format("Publishing order history to queue for message ({0}).", parsedFile.MessageId));
                logMsg.AppendLine();
				logMsg.AppendLine(jsonValue);

                _log.WriteInformationLog(logMsg.ToString());
            }
        }

        public void SocketPortClosed(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port closed");
        }

        public void SocketOpeningPort(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port opening");
        }

        public void SocketWaitingConnection(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener port connecting");
        }

        public void SocketBeginningFileReceipt(object sender, EventArgs e) {
            _log.WriteInformationLog("Order History listener beginning file receipt");
        }

        public void SocketExceptionEncountered(object sender, ExceptionEventArgs e) {
            _log.WriteErrorLog(string.Concat("Exception encountered in OrderHistoryLogic: ", e.Exception.Message));
            _log.WriteWarningLog("Listener will stop processing and will need to be restarted");

            KeithLink.Common.Impl.Email.ExceptionEmail.Send(e.Exception, "Listener will stop processing and will need to be restarted");
        }
        #endregion

        #region methods
        private void BuildAlertStringsForLostPurchaseOrders(out StringBuilder sbSubject, out StringBuilder sbBody, List<PurchaseOrder> Pos, string qStatus) {
            sbSubject = new StringBuilder();
            sbBody = new StringBuilder();
            if((Pos != null) && (Pos.Count > 0)) {
                int count = 0;
                sbBody.Clear();
                DateTime now = DateTime.Now.AddMinutes(-10);

                foreach(PurchaseOrder po in Pos) {
                    //string sCreated = po.Properties["DateCreated"].ToString();
                    DateTime created = DateTime.Parse(po.Properties["DateCreated"].ToString());
                    //// only if they've been created more than 10 minutes ago in the query status
                    if(created < now) {
                        count++;
                        sbSubject.Clear();
                        sbSubject.Append(count + " POs in a " + qStatus + " status for more than 10 minutes.");
                        if(sbBody.Length == 0)
                            sbBody.Append("Purchase Order Details:\n");
                        sbBody.Append("* PO");
                        sbBody.Append(" for ");
                        sbBody.Append(po.Properties["CustomerId"].ToString());
                        sbBody.Append("-");
                        sbBody.Append(po.Properties["BranchId"].ToString().ToUpper());
                        //sbBody.Append(" with cart ");
                        //sbBody.Append(po.Properties["Name"].ToString());
                        sbBody.Append(" with tracking ");
                        sbBody.Append(po.Properties["OrderNumber"].ToString());
                        sbBody.Append(" last modified");
                        sbBody.Append(" on " + created.ToString("MM-dd-yyyy hh:mm tt"));
                        sbBody.Append(" in status " + po.Properties["Status"].ToString());
                        sbBody.Append(".\n");
                    }
                }
            }
        }

        public string CheckForLostOrders(out string sBody) {
            StringBuilder sbMsgSubject = new StringBuilder();
            StringBuilder sbMsgBody = new StringBuilder();
            List<string> statuses = Configuration.CheckLostOrdersStatus;
            foreach(string status in statuses)
                CheckForLostOrdersByStatus(sbMsgSubject, sbMsgBody, status);
            sBody = sbMsgBody.ToString();
            if(sbMsgSubject.Length > 0)
                sbMsgSubject.Insert(0, "QSvc on " + Environment.MachineName + "; ");
            return sbMsgSubject.ToString();
        }

        private void CheckForLostOrdersByStatus(StringBuilder sbMsgSubject, StringBuilder sbMsgBody, string qStatus) {
            List<PurchaseOrder> Pos = _poRepo.GetPurchaseOrdersByStatus(qStatus);
            StringBuilder sbAppendSubject;
            StringBuilder sbAppendBody;
            BuildAlertStringsForLostPurchaseOrders(out sbAppendSubject, out sbAppendBody, Pos, qStatus);
            if(sbAppendSubject.Length > 0) {
                if(sbMsgSubject.Length > 0)
                    sbMsgSubject.Append(", ");
                sbMsgSubject.Append(sbAppendSubject.ToString());
            }
            if(sbAppendBody.Length > 0) {
                if(sbMsgBody.Length > 0)
                    sbMsgBody.Append("\n\n");
                sbMsgBody.Append(sbAppendBody.ToString());
            }
        }

        private void Create(OrderHistoryFile currentFile, bool isSpecialOrder) {
            // first attempt to find the order, look by confirmation number
            EF.OrderHistoryHeader header = null;

            if(!String.IsNullOrEmpty(currentFile.Header.ControlNumber) && !String.IsNullOrEmpty(currentFile.Header.OrderSystem.ToShortString())) {
                header = _headerRepo.ReadByConfirmationNumber(currentFile.Header.ControlNumber, currentFile.Header.OrderSystem.ToShortString()).FirstOrDefault();
            }

            // second attempt to find the order, look by invioce number
            if(header == null && !currentFile.Header.InvoiceNumber.Equals("Processing")) {
                header = _headerRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber).FirstOrDefault();
            }

            // last ditch effort is to create a new header
            if(header == null) {
                header = new EF.OrderHistoryHeader();
                header.OrderDetails = new List<EF.OrderHistoryDetail>();
            }

            currentFile.Header.MergeWithEntity(ref header);

            // set isSpecialOrder if that is true; but don't set otherwise (used from two places)
            if(isSpecialOrder) {
                header.IsSpecialOrder = true;
            }

            if(string.IsNullOrEmpty(header.OriginalControlNumber)) { header.OriginalControlNumber = currentFile.Header.ControlNumber; }

            bool hasSpecialItems = false;

            foreach(OrderHistoryDetail currentDetail in currentFile.Details.ToList()) {
                if(string.IsNullOrWhiteSpace(currentDetail.SpecialOrderHeaderId)) {
                    hasSpecialItems = true;
                }

                EF.OrderHistoryDetail detail = null;

                if(header.OrderDetails != null && header.OrderDetails.Count > 0) {
                    detail = header.OrderDetails.Where(d => (d.LineNumber == currentDetail.LineNumber)).FirstOrDefault();
                }

                if(detail == null) {
                    EF.OrderHistoryDetail tempDetail = currentDetail.ToEntityFrameworkModel();
                    tempDetail.BranchId = header.BranchId;
                    tempDetail.InvoiceNumber = header.InvoiceNumber;
                    tempDetail.OrderHistoryHeader = header;

                    if(isSpecialOrder) {
                        tempDetail.ItemStatus = KeithLink.Svc.Core.Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
                    }

                    header.OrderDetails.Add(tempDetail);
                } else {
                    currentDetail.MergeWithEntityFrameworkModel(ref detail);

                    detail.BranchId = header.BranchId;
                    detail.InvoiceNumber = header.InvoiceNumber;
                    if(isSpecialOrder) {
                        detail.ItemStatus = KeithLink.Svc.Core.Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
                    }
                }
            }

            _headerRepo.CreateOrUpdate(header);

            if(hasSpecialItems) {
                RemoveSpecialOrderItemsFromHistory(header);
            }
        }

        private void DetermineCatalogNotesSpecialOrder(PurchaseOrder po, ref EF.OrderHistoryHeader header) {
            //_log.WriteInformationLog("InternalOrderHistoryLogic.PullCatalogFromPurchaseOrderItemsToOrder() LineItems=" +
            //    ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"]).Count);
            string catalogId = null;
            string catalogType;
            if(po.Properties["LineItems"] != null) {
                foreach(var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)po.Properties["LineItems"])) {
                    var item = (CS.LineItem)lineItem.Target;
                    catalogId = item.CatalogName;
                    catalogType = _catalogLogic.GetCatalogTypeFromCatalogId(item.CatalogName);
                }
            }
            // Look for certain catalogs names or at least the start to be one of the special catalogs
            if(catalogId.IndexOf("unfi") > -1)
                header.IsSpecialOrder = true;
        }

        //public List<OrderHistoryHeader> GetCustomerOrderHistories(Core.Models.SiteCatalog.UserSelectedContext catalogInfo) {
        //    var histHeaders = _headerRepo.GetCustomerOrderHistoryHeaders(catalogInfo.BranchId, catalogInfo.CustomerId).ToList();
        //    // convert to orderhistoryheader from ef orderhistoryheader
        //    List<OrderHistoryHeader> orderHistories = new List<OrderHistoryHeader>();

        //    foreach(var histHeader in histHeaders) {
        //        OrderHistoryHeader retHistHeader = histHeader.ToOrderHistoryHeader();
        //        retHistHeader.Items = histHeader.OrderDetails.Select(l => l.ToOrderHistoryDetail()).ToList();
        //        orderHistories.Add(retHistHeader);
        //    }

        //    return orderHistories;
        //}


        /// <summary>
        /// Get a summary of order totals by month. Current month counts as 1.
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="numberOfMonths"></param>
        /// <returns></returns>
        public OrderTotalByMonth GetOrderTotalByMonth(UserSelectedContext customerInfo, int numberOfMonths) {
            OrderTotalByMonth returnValue = new OrderTotalByMonth();

            // Need to get the last day of the current month
            DateTime end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            // Need to get the first day from six months ago, create a new datetime object at the first of the current month
            // Subtract the numberOfMonths but add 1 as the current month is intended as one of the results
            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            start = start.AddMonths(-numberOfMonths + 1);

            try {
                List<Order> orders = GetShallowOrderDetailInDateRange(customerInfo, start, end);

                // Iterate through the buckets and grab the sum for that month
                for(int i = 0; i <= numberOfMonths - 1; i++) {
                    DateTime currentMonth = start.AddMonths(i);

                    double bucketValue = (from o in orders
                                          where o.CreatedDate.Month == currentMonth.Month
                                          select o.OrderTotal).DefaultIfEmpty(0).Sum();

                    returnValue.Totals.Add(bucketValue);
                }


                // Leaving this code commented out in case we need further performance increases
                //Parallel.For( 0, numberOfMonths - 1, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, i => {
                //    DateTime currentMonth = start.AddMonths( i );

                //    double bucketValue = (from o in orders
                //                          where o.CreatedDate.Month == currentMonth.Month
                //                          select o.OrderTotal).DefaultIfEmpty( 0 ).Sum();

                //    returnValue.Totals.Add( bucketValue );
                //} );

            } catch(Exception e) {
                _log.WriteErrorLog(String.Format("Error getting order total by month for customer: {0}, branch: {1}", customerInfo.CustomerId, customerInfo.BranchId), e);
                throw e;
            }

            return returnValue;
        }

        /// <summary>
        /// Gets just the primary order details needed without the excess of item details or invoice status
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private List<Order> GetShallowOrderDetailInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            int numberOfDays = (endDate - startDate).Days + 1;
            var dateRange = Enumerable.Range(0, numberOfDays).Select(d => startDate.AddDays(d).ToLongDateFormat());

            List<EF.OrderHistoryHeader> headers = _headerRepo.Read(h => h.BranchId.Equals(customerInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)
                                                                                                       && h.CustomerNumber.Equals(customerInfo.CustomerId)
                                                                                                       && dateRange.Contains(h.DeliveryDate),
                                                                                                    i => i.OrderDetails)
                                                                                           .ToList();
            List<Order> orders = new List<Order>();

            foreach(EF.OrderHistoryHeader h in headers) {
                Order order = h.ToOrder();

                if(order.Items != null) {
                    order.OrderTotal = order.Items.Sum(i => i.LineTotal);
                }

                orders.Add(order);
            }

            // Leaving this code commented out in case we need further performance increases
            //Parallel.ForEach( headers, new ParallelOptions { MaxDegreeOfParallelism = 2 }, h => {
            //    Order order = h.ToOrder();

            //    if (order.Items != null) {
            //        order.OrderTotal = order.Items.AsParallel().WithDegreeOfParallelism(2).Sum(i => i.LineTotal);
            //    }

            //    orders.Add( order );
            //} );

            return orders;
        }

        public void ListenForMainFrameCalls() {
            _socket.Listen(Configuration.MainframOrderHistoryListeningPort);
        }

        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while(_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try {
                    var rawOrder = ReadOrderFromQueue();

                    while(_keepListening && !string.IsNullOrEmpty(rawOrder)) {
                        OrderHistoryFile historyFile = new OrderHistoryFile();

                        historyFile = JsonConvert.DeserializeObject<OrderHistoryFile>(rawOrder);

                        _log.WriteInformationLog(string.Format("Consuming order update from queue for message ({0})", historyFile.MessageId));

                        Create(historyFile, false);
                        _conversionLogic.SaveOrderHistoryAsConfirmation(historyFile);

                        _unitOfWork.SaveChangesAndClearContext();

                        // to make sure we do not pull an order off the queue without processing it
                        // check to make sure we can still process before pulling off the queue
                        if(_keepListening) {
                            rawOrder = ReadOrderFromQueue();
                        } else {
                            rawOrder = null;
                        }
                    }
                } catch(Exception ex) {
                    KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex, subject: "Exception processing Order History in Queue Service");

                    _log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }
        }

        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private OrderHistoryFileReturn ParseFile(string[] data) {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            OrderHistoryFile currentFile = null;

            for(int i = 0; i < data.Length; i++) {
                if(data[i].Contains("END###")) { break; }

                switch(data[i].Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                    case "H":
                        if(currentFile != null) {
                            currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                              where detail.ItemStatus != string.Empty
                                                              select true).FirstOrDefault();
                            currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                              where detail.FutureItem == true
                                                              select true).FirstOrDefault();
                            retVal.Files.Add(currentFile);
                        }

                        currentFile = new OrderHistoryFile();

                        currentFile.Header.Parse(data[i]);
                        break;
                    case "D":
                        if(currentFile != null) {
                            OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                            orderDetail.Parse(data[i]);

                            currentFile.Details.Add(orderDetail);
                        }
                        break;
                    default:
                        break;
                }

            } // end of for loop

            if(currentFile != null) {
                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                  where detail.ItemStatus != string.Empty
                                                  select true).FirstOrDefault();
                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                  where detail.FutureItem == true
                                                  select true).FirstOrDefault();
                retVal.Files.Add(currentFile);
            }

            return retVal;
        }

        public OrderHistoryFileReturn ParseMainframeFile(string filePath) {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            using(System.IO.TextReader txtFile = System.IO.File.OpenText(filePath)) {
                OrderHistoryFile currentFile = null;

                while(txtFile.Peek() != -1) {
                    string data = txtFile.ReadLine();

                    switch(data.Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                        case "H":
                            if(currentFile != null) {
                                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.ItemStatus != string.Empty
                                                                  select true).FirstOrDefault();
                                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.FutureItem == true
                                                                  select true).FirstOrDefault();
                                retVal.Files.Add(currentFile);
                            }

                            currentFile = new OrderHistoryFile();

                            // check for length of header record to make sure there is data
                            if(data.Trim().Length > 1) {
                                try {
                                    currentFile.Header.Parse(data);
                                    currentFile.ValidHeader = true;
                                } catch {
                                    currentFile.ValidHeader = false;
                                }
                            } else {
                                currentFile.ValidHeader = false;
                            }

                            break;
                        case "D":
                            if(currentFile != null) {
                                OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                                orderDetail.Parse(data);

                                currentFile.Details.Add(orderDetail);
                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if(currentFile != null) {
                    currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.ItemStatus != string.Empty
                                                      select true).FirstOrDefault();
                    currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.FutureItem == true
                                                      select true).FirstOrDefault();
                    retVal.Files.Add(currentFile);
                }
            }

            return retVal;
        }

        private string ReadOrderFromQueue() {
            return _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer,
                                           Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueHourlyUpdates);
        }

        private void RemoveEmptyPurchaseOrder() { }

        private void RemoveSpecialOrderItemsFromHistory(EF.OrderHistoryHeader order) {
            // clean up any previous orders where the special order item existed
            var specialOrderInfo = order.OrderDetails.Where(currentDetail => !String.IsNullOrEmpty(currentDetail.SpecialOrderHeaderId))
                                                     .Select(d => new { HeaderId = d.SpecialOrderHeaderId, LineNumber = d.SpecialOrderLineNumber })
                                                     .Distinct()
                                                     .ToList();

            // loop through each special order item in the current order
            foreach(var specialOrderItem in specialOrderInfo) {
                // find all detail records with the current line's special order info that is not the current order
                var specialLines = _detailRepo.Read(d => d.BranchId.Equals(order.BranchId)
                                                      && d.SpecialOrderHeaderId.Equals(specialOrderItem.HeaderId)
                                                      && d.SpecialOrderLineNumber.Equals(specialOrderItem.LineNumber)
                                                      && !d.InvoiceNumber.Equals(order.InvoiceNumber))
                                              .ToList();

                // loop through each found detail record
                foreach(var line in specialLines) {
                    _detailRepo.Delete(line);

                    // check to see if there are any more records on the detail's header record
                    if(_detailRepo.Read(d => d.BranchId.Equals(line.BranchId)
                                          && d.InvoiceNumber.Equals(line.InvoiceNumber))
                                  .Any() == false) {
                        _headerRepo.Delete(h => h.BranchId.Equals(line.BranchId)
                                             && h.InvoiceNumber.Equals(line.InvoiceNumber));
                    }
                }
            }

            // this is commented out so that all updates to EF happen in one transaction for the current order
            //_unitOfWork.SaveChanges();
        }

        public void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder) {
            Create(historyFile, isSpecialOrder);

            _unitOfWork.SaveChanges();
        }

        public string SetLostOrder(string trackingNumber) {
            _log.WriteInformationLog("InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ")");
            PurchaseOrder Po = _poRepo.ReadPurchaseOrderByTrackingNumber(trackingNumber);
            //Save to Commerce Server
            if(Po != null) {
                com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
                client.UpdatePurchaseOrderStatus(Po.Properties["UserId"].ToString().ToGuid(), Po.Id.ToGuid(), "Lost");
                _log.WriteInformationLog(" InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ") Success");
                return "Success";
            } else {
                _log.WriteInformationLog(" InternalOrderHistoryLogic.SetLostOrder(trackingNumber=" + trackingNumber + ") Po not found");
                return "Po not found";
            }
        }

        public void StopListening() {
            _keepListening = false;

            if(_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }

        public void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber) {
            var header = _headerRepo.ReadByConfirmationNumber(childOrderNumber, "B").FirstOrDefault();

            if(header != null) {
                header.RelatedControlNumber = parentOrderNumber;

                _headerRepo.Update(header);

                _unitOfWork.SaveChanges();
            }
        }
        #endregion
    }
}
