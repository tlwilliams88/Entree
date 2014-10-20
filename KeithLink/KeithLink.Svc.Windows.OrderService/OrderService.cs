 using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Confirmations;
using System;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Text;

namespace KeithLink.Svc.Windows.OrderService {
    partial class OrderService : ServiceBase {

        #region attributes

        private EventLogRepositoryImpl _log;
        private static bool _processing;
        private static bool _successfulConnection;
        private static bool _confirmationMoverProcessing;

        private static UInt16 _unsentCount;

        private Timer _queueTimer;
        private Timer _confirmationMover;
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
            _processing = false;
            _successfulConnection = false;
            _unsentCount = 0;
            _confirmationMoverProcessing = false;
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
            KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, msg.ToString());
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
            //AutoResetEvent auto = new AutoResetEvent( true );
            //TimerCallback cb = new TimerCallback( MoveConfirmationsToCommerceServiceTick );

            //_confirmationMover = new Timer( cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK );
        }

        private void InitializeConfirmationThread()
        {
            _confirmationThread = new Thread(ProcessConfirmations);
            _confirmationThread.Start();
        }

        private void InitializeQueueTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessQueueTick);

            _queueTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void MoveConfirmationsToCommerceServiceTick() {
            // TODO : Add logic for moving to Commerce Server
        }

        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

            InitializeQueueTimer();
            InitializeConfirmationThread();
        }

        protected override void OnStop() {
            TerminateQueueTimer();

            _log.WriteInformationLog("Service stopping");
        }

        protected void ProcessConfirmations() {
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

        private void ProcessQueueTick(object state) {
            if (!_processing) {
                _processing = true;

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

                _processing = false;
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
