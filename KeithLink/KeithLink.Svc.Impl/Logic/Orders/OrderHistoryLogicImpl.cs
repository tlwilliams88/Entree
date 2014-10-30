using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryLogicImpl : IOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;
        private const int THREAD_SLEEP_DURATION = 2000;

        private readonly IEventLogRepository _log;
        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IOrderHistoryQueueRepository _queue;
        private readonly IUnitOfWork _unitOfWork;

        private bool _keepListening;
        private Task _queueTask;
        #endregion

        #region ctor
        public OrderHistoryLogicImpl(IEventLogRepository logRepo, IOrderHistoryHeaderRepsitory headerRepo, IOrderHistoryDetailRepository detailRepo, IOrderHistoryQueueRepository queueRepo,
                                     IUnitOfWork unitOfWork) {
            _log = logRepo;
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
            _queue = queueRepo;
            _unitOfWork = unitOfWork;

            _keepListening = true;
        }
        #endregion

        #region methods
        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while (_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try {
                    string rawOrder = _queue.ConsumeFromQueue();

                    while (rawOrder != null) {
                        OrderHistoryFile historyFile = new OrderHistoryFile();

                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(historyFile.GetType());
                        System.IO.StringReader xmlData = new System.IO.StringReader(rawOrder);

                        historyFile = (OrderHistoryFile)xs.Deserialize(xmlData);

                        Save(historyFile);

                        rawOrder = _queue.ConsumeFromQueue();

                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }

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

                            currentFile.Header.Parse(data);
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

        public void Save(OrderHistoryFile currentFile) {
            IEnumerable<EF.OrderHistoryHeader> headers = _headerRepo.ReadForInvoice(currentFile.Header.BranchId, currentFile.Header.InvoiceNumber);
            EF.OrderHistoryHeader tempheader = headers.FirstOrDefault();

            EF.OrderHistoryHeader header = currentFile.Header.ToEntityFrameworkModel();

            header.OrderDetails = new List<EF.OrderHistoryDetail>();

            foreach (OrderHistoryDetail item in currentFile.Details) {
                header.OrderDetails.Add(item.ToEntityFrameworkModel());
            }

            _headerRepo.CreateOrUpdate(header);

            _unitOfWork.SaveChanges();
        }

        public void StopListening() {
            _keepListening = false;

            if (_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }
        #endregion
    }
}
