using KeithLink.Common.Core.Email;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Confirmations;
using KeithLink.Svc.Impl.Repository.Orders.History;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Text;
using System.Xml.Serialization;

namespace KeithLink.Svc.Windows.OrderService {
    partial class OrderService : ServiceBase {

        #region attributes
        private EventLogRepositoryImpl _log;

        private static bool _allowOrderUpdateProcessing;
        private static bool _confirmationMoverProcessing;
        private static bool _orderUpdateProcessing;
        private static bool _orderQueueProcessing;
        private static bool _successfulConnection;
        private static bool _silenceOrderUpdateMessages;

        private static UInt16 _unsentCount;

        private Timer _queueTimer;
        private Timer _confirmationMover;
        private Timer _orderUpdateTimer;
        private Thread _confirmationThread;

        const int TIMER_DURATION_TICK = 2000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public OrderService() {
            InitializeComponent();

            _log = new EventLogRepositoryImpl(this.ServiceName);
            _orderQueueProcessing = false;
            _successfulConnection = false;
            _unsentCount = 0;
            _confirmationMoverProcessing = false;
            _orderUpdateProcessing = false;

            _allowOrderUpdateProcessing = true;
            _silenceOrderUpdateMessages = false;
        }
        #endregion

        #region methods
        private void HandleCancelledException(CancelledTransactionException ex) {
            StringBuilder msg = new StringBuilder();

            msg.AppendLine();
            msg.Append("Transaction cancelled by host while processing a file. This file should resume processing again almost immediately. ");
            msg.Append("This could be caused by the AR001v (customer master) being closed. The helpdesk should have been notified on the console if that is the case. ");
            msg.Append("Other files being closed could also be the problem. Please refer to transaction ");
            msg.Append(KeithLink.Svc.Impl.Configuration.MainframeOrderTransactionId);
            msg.AppendLine(" for further troubleshooting.");
            msg.AppendLine();
            msg.AppendLine("Confirmation Number: ");
            msg.AppendLine(ex.ConfirmationNumber.ToString());

            _log.WriteInformationLog(msg.ToString());
            ExceptionEmail.Send(ex, msg.ToString());
        }

        private void HandleEarlySocketException(Exception ex) {
            if (_successfulConnection) {
                _successfulConnection = false;

                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Error beginning communications.");
                msg.AppendLine();
                msg.AppendLine("******************************************************************************");
                msg.AppendLine("            If this message was received while CICS was unavailable, ");
                msg.AppendLine("            please disregard this email. The affected files will begin ");
                msg.AppendLine("            processing when CICS becomes available.");
                msg.AppendLine("******************************************************************************");
                msg.AppendLine();

                _log.WriteErrorLog(msg.ToString());
                KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, msg.ToString());
            }
        }

        private void HandleMissingOrderUpdateWatchPath() {
            StringBuilder addMsg = new StringBuilder();
            addMsg.Append("The OrderUpdateWatchPath is currently set to ");
            addMsg.Append(Configuration.OrderUpdateWatchPath);
            addMsg.Append(". This path does not exist on the server. This email will not be sent again until the service restarts. ");
            addMsg.Append("The path can be added with the service running and the service will pick it up on the next attempt. ");
            addMsg.AppendLine();
            addMsg.AppendLine();
            addMsg.Append("This will affect order updates within the E-Commerce application. It is low priority. The helpdesk should not ");
            addMsg.Append("create an incident, but should contact the on call person from the E-Commerce group.");

            ExceptionEmail.Send(new ApplicationException("Path specified by OrderUpdateWatchPath does not exist"), addMsg.ToString());
        }

        private void HandleException(Exception ex) {
            StringBuilder msg = new StringBuilder();

            msg.AppendLine();
            msg.Append("An unhandled exception was detected in the KeithLink Order Service. Please contact the on-call person in the E-Business group and open an incident.");

            _log.WriteErrorLog(msg.ToString(), ex);
            ExceptionEmail.Send(ex, msg.ToString());
        }

        private void HandleSocketResponseException(Exception ex) {
            if (_successfulConnection && _unsentCount == 5) {
                _successfulConnection = false;

                StringBuilder msg = new StringBuilder();
                msg.AppendLine();
                msg.AppendLine();
                msg.AppendLine("******************************************************************************");
                msg.AppendLine("        Timed out waiting for response from mainframe after 5 reties.");
                msg.AppendLine("******************************************************************************");
                msg.AppendLine();
                msg.AppendLine();

                _log.WriteErrorLog(msg.ToString());
                KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, msg.ToString());
            } else {
                _unsentCount++;
            }
        }

        private void InitializeConfirmationMoverThread() {
            AutoResetEvent auto = new AutoResetEvent( true );
            TimerCallback cb = new TimerCallback( MoveConfirmationsToCommerceServiceTick );

            _confirmationMover = new Timer( cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK );
        }

        private void InitializeConfirmationThread()
        {
            _confirmationThread = new Thread(ProcessConfirmations);
            _confirmationThread.Start();
        }

        private void InitializeOrderUpdateTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessOrderUpdatesTick);

            _orderUpdateTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void InitializeQueueTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessQueueTick);

            _queueTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void MoveConfirmationsToCommerceServiceTick(object state) {
            if (!_confirmationMoverProcessing) {
                _confirmationMoverProcessing = true;

                try {
                    //
                    ConfirmationLogicImpl confirmationLogic = new ConfirmationLogicImpl(_log,
                                                                    new KeithLink.Svc.Impl.Repository.Confirmations.ConfirmationListenerRepositoryImpl(),
                                                                    new KeithLink.Svc.Impl.Repository.Confirmations.ConfirmationQueueRepositoryImpl());

                    confirmationLogic.ProcessQueued();
                } catch (Exception e) {
                    //HandleConfirmationQueueProcessingerror(e);
                }

            }
        }

        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

            InitializeConfirmationThread();
            InitializeOrderUpdateTimer();
            InitializeQueueTimer();
        }

        protected override void OnStop() {
            TerminateOrderUpdateTimer();
            TerminateQueueTimer();

            _log.WriteInformationLog("Service stopping");
        }

        private void ProcessConfirmations() {
            try {
                ConfirmationLogicImpl confirmationLogic = new ConfirmationLogicImpl(_log,
                                                                       new KeithLink.Svc.Impl.Repository.Confirmations.ConfirmationListenerRepositoryImpl(),
                                                                       new KeithLink.Svc.Impl.Repository.Confirmations.ConfirmationQueueRepositoryImpl());
                confirmationLogic.Listen();
            }
            catch (Exception e) {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Processing failed receiving confirmation. ");
                Exception currentException = e;

                while (currentException != null) {
                    logMessage.AppendLine("Message:");
                    logMessage.AppendLine(currentException.Message);
                    logMessage.AppendLine();
                    logMessage.AppendLine("Stack:");
                    logMessage.AppendLine(currentException.StackTrace);

                    currentException = currentException.InnerException;
                }
                
                _log.WriteErrorLog(logMessage.ToString());

                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, logMessage.ToString());
            }
        }

        private void ProcessOrderUpdatesTick(object state) {
            if (!_orderUpdateProcessing) {
                _orderUpdateProcessing = true;

                // if update path does not exist, send an email then suppress messages
                // disallow processing here until the folder exists, but do not kill the service
                // to keep other threads running
                if (Directory.Exists(Configuration.OrderUpdateWatchPath)) {
                    _allowOrderUpdateProcessing = true;
                    _silenceOrderUpdateMessages = false;
                } else {
                    _allowOrderUpdateProcessing = false;

                    if (!_silenceOrderUpdateMessages) {
                        HandleMissingOrderUpdateWatchPath();

                        _silenceOrderUpdateMessages = true;
                    }
                }

                if (_allowOrderUpdateProcessing) {
                    string[] files = Directory.GetFiles(Configuration.OrderUpdateWatchPath);

                    foreach (string filePath in files) {
                        OrderHistoryLogicImpl logic = new OrderHistoryLogicImpl();

                        OrderHistoryFileReturn parsedFile = logic.ParseMainframeFile(filePath);

                        foreach (OrderHistoryFile file in parsedFile.Files) {
                            try {
                                StringWriter xmlWriter = new StringWriter();
                                XmlSerializer xs = new XmlSerializer(file.GetType());

                                xs.Serialize(xmlWriter, file);
                                
                                OrderUpdateQueueRepositoryImpl repo = new OrderUpdateQueueRepositoryImpl();
                                repo.PublishToQueue(xmlWriter.ToString());

                                _silenceOrderUpdateMessages = false;
                            } catch (Exception ex) {
                                if (!_silenceOrderUpdateMessages) {
                                    HandleException(ex);
                                    _silenceOrderUpdateMessages = true;
                                    break;
                                }
                            }
                        }

                        System.IO.File.Delete(filePath);
                    }
                }

                _orderUpdateProcessing = false;
            }
        }

        private void ProcessQueueTick(object state) {
            if (!_orderQueueProcessing) {
                _orderQueueProcessing = true;

                // do not process between 1 and 5
                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5) {
                    _log.WriteInformationLog("Script stopped for processing window");

                    while (DateTime.Now.Hour < 5) {
                        System.Threading.Thread.Sleep(60000);
                    }

                    _log.WriteInformationLog("Script started after processing window");
                }

                try {
                    OrderQueueLogicImpl orderQueue = new OrderQueueLogicImpl(_log,
                                                                   new KeithLink.Svc.Impl.Repository.Orders.OrderQueueRepositoryImpl(),
                                                                   new KeithLink.Svc.Impl.Repository.Orders.OrderSocketConnectionRepositoryImpl());
                    orderQueue.ProcessOrders();

                    _successfulConnection = true;
                    _unsentCount = 0;
                } catch(EarlySocketException earlyEx){
                    HandleEarlySocketException(earlyEx);
                } catch(SocketResponseException responseEx){
                    HandleSocketResponseException(responseEx);
                } catch(CancelledTransactionException cancelledEx){
                    HandleCancelledException(cancelledEx);
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error processing orders", ex);
                    KeithLink.Common.Core.Email.ExceptionEmail.Send(ex);
                }

                _orderQueueProcessing = false;
            }
        }


        private void TerminateOrderUpdateTimer() {
            if (_orderUpdateTimer != null) {
                _orderUpdateTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }
        }

        private void TerminateQueueTimer() {
            if (_queueTimer != null) {
                _queueTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }
        }

        #endregion
    }
}
