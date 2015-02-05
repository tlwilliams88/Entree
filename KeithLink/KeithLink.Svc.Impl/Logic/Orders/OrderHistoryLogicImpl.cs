using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Enumerations;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryLogicImpl : IOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;
        
        private readonly IEventLogRepository _log;
        private readonly IGenericQueueRepository _queue;
        private readonly ISocketListenerRepository _socket;
        
        #endregion

        #region ctor
        public OrderHistoryLogicImpl(IEventLogRepository logRepo, IGenericQueueRepository queueRepo, ISocketListenerRepository socket)
		{
            _log = logRepo;
            _queue = queueRepo;
            _socket = socket;            

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

            KeithLink.Common.Core.Email.ExceptionEmail.Send(e.Exception, "Listener will stop processing and will need to be restarted");
        }
        #endregion

        #region methods
        
		
        public void ListenForMainFrameCalls() {
            _socket.Listen(Configuration.MainframOrderHistoryListeningPort);
        }

        
        /// <summary>
        /// Parse an array of strings as a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private OrderHistoryFileReturn ParseFile(string[] data) {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            OrderHistoryFile currentFile = null;

            for (int i = 0; i < data.Length; i++) {
                if (data[i].Contains("END###")) { break; }

                switch (data[i].Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                    case "H":
                        if (currentFile != null) {
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
                        if (currentFile != null) {
                            OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                            orderDetail.Parse(data[i]);

                            currentFile.Details.Add(orderDetail);
                        }
                        break;
                    default:
                        break;
                }

            } // end of for loop

            if (currentFile != null) {
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

        public OrderHistoryFileReturn ParseMainframeFile(string filePath)
        {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            using (System.IO.TextReader txtFile = System.IO.File.OpenText(filePath)) {
                OrderHistoryFile currentFile = null;

                while (txtFile.Peek() != -1) {
                    string data = txtFile.ReadLine();

                    switch (data.Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                        case "H":
                            if (currentFile != null) {
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
                            if (data.Trim().Length > 1) {
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
                            if (currentFile != null) {
                                OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                                orderDetail.Parse(data);

                                currentFile.Details.Add(orderDetail);
                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if (currentFile != null) {
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

        #endregion

    }
}
