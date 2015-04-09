using KeithLink.Common.Core.Email;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Extensions.SingleSignOn;
using KeithLink.Svc.Core.Interface.SingleSignOn;
using KeithLink.Svc.Core.Interface.PowerMenu;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.SingleSignOn;
using KeithLink.Svc.Core.Models.PowerMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.SingleSignOn {
    public class AccessRequestLogicImpl : IAccessRequestLogic {
        #region attributes
        private const int DURATION_TIMER_INTERVAL = 5000;

        private IKbitRequestLogic _kbitLogic;
        private IPowerMenuLogic _pmLogic;
        private bool _keepQueueListening;
        private IEventLogRepository _log;
        private IGenericQueueRepository _queue;
        private Task _queueListenerTask;
        #endregion

        #region ctor
        public AccessRequestLogicImpl(IGenericQueueRepository queueRepo, IKbitRequestLogic KbitRequestLogic, IEventLogRepository logRepo, IPowerMenuLogic pmLogic) {
            _kbitLogic = KbitRequestLogic;
            _pmLogic = pmLogic;
            _keepQueueListening = true;
            _log = logRepo;
            _queue = queueRepo;
        }
        #endregion

        #region methods
        private BaseAccessRequest PopRequestFromQueue() {
            string queuedRequest = _queue.ConsumeFromQueue(Configuration.RabbitMQAccessServer, Configuration.RabbitMQAccessUserNameConsumer, Configuration.RabbitMQAccessUserPasswordConsumer,
                                                           Configuration.RabbitMQVHostAccess, Configuration.RabbitMQQueueAccess);
            if (queuedRequest == null) {
                return null;
            } else {
                return queuedRequest.Deserialize();
            }
        }

        public void ProcessQueuedRequests() {
            _queueListenerTask = new Task(WorkQueuedRequests);
            _queueListenerTask.Start();
        }

        public void StopProcessing(){
            _keepQueueListening = false;
            if (_queueListenerTask != null && _queueListenerTask.Status == TaskStatus.Running)
                _queueListenerTask.Wait();        
        }

        private void WorkQueuedRequests() {
            while (_keepQueueListening) {
                try {
                    System.Threading.Thread.Sleep(DURATION_TIMER_INTERVAL);

                    BaseAccessRequest request = PopRequestFromQueue();

                    if (request != null) {
                        try {
                            _log.WriteInformationLog(string.Concat("Processing request for ", request.UserName));

                            switch (request.RequestType) {
                                case Core.Enumerations.SingleSignOn.AccessRequestType.KbitCustomer:
                                    _kbitLogic.UpdateUserAccess(request.UserName, ((KbitCustomerAccessRequest)request).Customers);
                                    break;
                                case Core.Enumerations.SingleSignOn.AccessRequestType.PowerMenu:
                                    _pmLogic.SendAccountRequestToPowerMenu( ((PowerMenuCustomerAccessRequest)request).UserName );
                                    break;
                                default:
                                    throw new ApplicationException("Unknown request type received.");
                            }

                            _log.WriteInformationLog(string.Concat("Completed processing for ", request.UserName));
                        } catch (Exception innerEx) {
                            string exMsg = string.Join("Could not process request.\n\nRequest:", request.ToJSON());

                            _log.WriteErrorLog(exMsg, innerEx);
                            ExceptionEmail.Send(innerEx, exMsg);
                        }
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Unhandled exception encountered.", ex);
                    ExceptionEmail.Send(ex);
                }
            }
        }
        #endregion
    }
}
