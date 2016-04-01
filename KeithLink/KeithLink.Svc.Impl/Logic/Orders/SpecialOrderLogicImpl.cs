using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Email;

using KeithLink.Svc.Core;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;

using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SpecialOrders;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class SpecialOrderLogicImpl : ISpecialOrderLogic {
        #region attributes
        private const int THREAD_SLEEP_DURATION = 2000;

        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventLogRepository _log;
        private readonly IGenericQueueRepository _queue;

        private bool _keepListening;
        private Task _queueTask;
        #endregion

        #region ctor
        public SpecialOrderLogicImpl(IUnitOfWork unitOfWork, IEventLogRepository log, IGenericQueueRepository queue, 
                                     IOrderHistoryDetailRepository detailRepo, IOrderHistoryHeaderRepsitory headerRepo) {
            _unitOfWork = unitOfWork;
            _log = log;
            _queue = queue;
            _keepListening = true;
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
        }
        #endregion

        #region methods
        // queue listener methods
        public void ListenForQueueMessages() {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask());
        }

        private void ListenForQueueMessagesInTask() {
            while(_keepListening) {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try {
                    string rawOrder = ReadOrderFromQueue();

                    // for testing; create a sample specialorderresponse
                    //rawOrder = CreateTestSample();

                    while(_keepListening && !string.IsNullOrEmpty(rawOrder)) {
                        ProcessSpecialOrderItemUpdate(rawOrder);
                        // to make sure we do not pull an order off the queue without processing it
                        // check to make sure we can still process before pulling off the queue
                        if(_keepListening) {
                            rawOrder = ReadOrderFromQueue();
                        } else {
                            rawOrder = null;
                        }
                    }
                } catch(Exception ex) {
                    ExceptionEmail.Send(ex, subject: "Exception processing SpecialOrderLogic.ListenForQueueMessagesInTask");

                    _log.WriteErrorLog("Error in SpecialOrderLogic.ListenForQueueMessagesInTask ", ex);
                }
            }
        }

        private static string CreateTestSample() {
            SpecialOrderResponseModel specialorder = new SpecialOrderResponseModel();
            specialorder.Header = new ResponseHeader();
            specialorder.Item = new ResponseItem();
            return JsonConvert.SerializeObject(specialorder);
        }

        private void ProcessSpecialOrderItemUpdate(string rawOrder) {
            SpecialOrderResponseModel specialorder = new SpecialOrderResponseModel();
            specialorder = JsonConvert.DeserializeObject<SpecialOrderResponseModel>(rawOrder);

            while((specialorder != null) && (specialorder.MessageId != null) && (specialorder.Header.RequestHeaderId != null) && (specialorder.Item.LineNumber != null)) {
                _log.WriteInformationLog(string.Format("Consuming specialorder update from queue for message ({0}) with status {1}", specialorder.MessageId, specialorder.Item.ItemStatusId));

                EF.OrderHistoryDetail detail = FindOrderHistoryDetailForUpdate(specialorder);

                if(detail != null) // only process if we match the order specified on this system
                {
                    ProcessOrderHistoryDetailByUpdateStatus(specialorder, detail);
                } else {
                    _log.WriteInformationLog(string.Format(" ({0}) Specialorder update from queue for message not an order on this system", specialorder.MessageId));
                }
            }
        }

        private EF.OrderHistoryDetail FindOrderHistoryDetailForUpdate(SpecialOrderResponseModel specialorder) {
            EF.OrderHistoryDetail detail = null;

            // try to find detail by specialorderheaderid and linenumber
            if(!String.IsNullOrEmpty(specialorder.Header.RequestHeaderId) && !String.IsNullOrEmpty(specialorder.Item.LineNumber)) {
                detail = _detailRepo.Read(d => d.SpecialOrderHeaderId == specialorder.Header.RequestHeaderId &&
                                               d.SpecialOrderLineNumber == specialorder.Item.LineNumber).FirstOrDefault();
            }
            return detail;
        }

        private void ProcessOrderHistoryDetailByUpdateStatus(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail) {
            switch(specialorder.Item.ItemStatusId) {
                case Constants.SPECIALORDERITEM_DEL_STATUS_CODE:
                case Constants.SPECIALORDERITEM_PUR_STATUS_CODE: {
                        ProcessOrderHistoryDetailOnDeletedOrPurgedStatusUpdate(detail);
                        break;
                    }
                case Constants.SPECIALORDERITEM_APP_STATUS_CODE: {
                        ProcessOrderHistoryDetailOnApprovedStatusUpdate(specialorder, detail);
                        break;
                    }
                    //default: // not exposing any status from KSOS right now
                    //    {
                    //        ProcessOrderHistoryDetailOnStatusUpdate(specialorder, detail);
                    //        break;
                    //    }
            }
        }

        private void ProcessOrderHistoryDetailOnDeletedOrPurgedStatusUpdate(EF.OrderHistoryDetail detail) {
            EF.OrderHistoryHeader header = detail.OrderHistoryHeader;
            if(header != null) {
                if(header.OrderDetails.Count > 1) {
                    _detailRepo.Delete(detail);
                } else {
                    _headerRepo.Delete(header);
                }
                _unitOfWork.SaveChanges();
            } else {
                _detailRepo.Delete(detail);
                _unitOfWork.SaveChanges();
            }
        }

        private void ProcessOrderHistoryDetailOnApprovedStatusUpdate(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail) {
            _log.WriteInformationLog(string.Format(" ({0}) InternalSpecialOrderLogic.ProcessOrderHistoryDetailOnApprovedStatusUpdate", specialorder.MessageId));
            EF.OrderHistoryHeader header = detail.OrderHistoryHeader;
            header.DeliveryDate = specialorder.Item.EstimatedArrival;
            _headerRepo.Update(header);
            _unitOfWork.SaveChanges();
        }

        private void ProcessOrderHistoryDetailOnStatusUpdate(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail) {
            _log.WriteInformationLog(string.Format(" ({0})  InternalSpecialOrderLogic.ProcessOrderHistoryDetailOnStatusUpdate", specialorder.MessageId));
            switch(specialorder.Item.ItemStatusId) {
                case Constants.SPECIALORDERITEM_NEW_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_NEW_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_ERR_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_ERR_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_2MF_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_2MF_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_REQ_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_ACC_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_ACC_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_APP_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_APP_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_DEL_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_DEL_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_HLD_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_HLD_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_RCV_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_RCV_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_R_H_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_R_H_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_ATT_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_ATT_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_PTL_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_PTL_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_SHP_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_SHP_STATUS_TRANSLATED_CODE;
                    break;
                case Constants.SPECIALORDERITEM_PUR_STATUS_CODE:
                    detail.ItemStatus = Constants.SPECIALORDERITEM_PUR_STATUS_TRANSLATED_CODE;
                    break;
            }
            _detailRepo.Update(detail);
            _unitOfWork.SaveChanges();
        }

        private string ReadOrderFromQueue() {
            return _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer,
                                           Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueSpecialOrderUpdateRequests);
        }

        public void StopListening() {
            _keepListening = false;

            if(_queueTask != null && _queueTask.Status == TaskStatus.Running) {
                _queueTask.Wait();
            }
        }
        #endregion
    }
}
