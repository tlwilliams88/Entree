﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Impl.Email;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl;

using Autofac;
using Newtonsoft.Json;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace KeithLink.Svc.Windows.OrderService
{
    public partial class OrderService : ServiceBase
    {

        #region attributes
        private IContainer _diContainer;
        private IEventLogRepository _log;
        private IOrderHistoryWriter _orderHistoryWriter;

        private ILifetimeScope _confirmationScope;
        private ILifetimeScope _historyRequestScope;
        private ILifetimeScope _historyResponseScope;
        private ILifetimeScope _orderScope;
        private ILifetimeScope _queueScope;
        private ILifetimeScope _orderHistoryWriterScope;

        private static bool _allowOrderUpdateProcessing;
        private static bool _historyRequestProcessing;
        private static bool _orderUpdateProcessing;
        private static bool _orderQueueProcessing;
        private static bool _successfulHistoryConnection;
        private static bool _successfulOrderConnection;
        private static bool _silenceOrderUpdateMessages;

        private static UInt16 _unsentCount;

        private Thread _confirmationThread;
        private Thread _historyResponseThread;
        private Timer _historyRequestTimer;
        private Timer _orderUpdateTimer;
        private Timer _queueTimer;

        const int TIMER_DURATION_TICK = 2000;
        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public OrderService()
        {
            _diContainer = Impl.Repository.SmartResolver.DependencyMapFactory.GetOrderServiceContainer().Build();

            InitializeComponent();

            _log = _diContainer.Resolve<IEventLogRepository>();

            _orderQueueProcessing = false;
            _successfulHistoryConnection = true;
            _successfulOrderConnection = true;
            _unsentCount = 0;
            _orderUpdateProcessing = false;

            _allowOrderUpdateProcessing = true;
            _silenceOrderUpdateMessages = false;
        }
        #endregion

        #region methods
        private bool CanOpenFile(string filePath)
        {
            bool opened = false;
            int loopCnt = 0;

            while (loopCnt < 30 && !opened)
            {
                try
                {
                    FileStream myFile = System.IO.File.OpenWrite(filePath);
                    myFile.Close();
                    myFile.Dispose();

                    opened = true;
                }
                catch
                {
                    Thread.Sleep(1000);
                    loopCnt++;
                }
            }

            return opened;
        }

        private void HandleCancelledException(CancelledTransactionException ex)
        {
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

        private void HandleEarlySocketException(Exception ex, bool historyConnection)
        {
            if ((historyConnection && _successfulHistoryConnection) || (!historyConnection && _successfulOrderConnection))
            {
                if (historyConnection)
                {
                    _successfulHistoryConnection = false;
                }
                else
                {
                    _successfulOrderConnection = false;
                }

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
                ExceptionEmail.Send(ex, msg.ToString());
            }

            // wait for one minute before allowing the service to continue
            Thread.Sleep(60000);
        }

        private void HandleMissingOrderUpdateWatchPath()
        {
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

        private void HandleException(Exception ex)
        {
            StringBuilder msg = new StringBuilder();

            msg.AppendLine();
            msg.Append("An unhandled exception was detected in the KeithLink Order Service. Please contact the on-call person in the E-Business group and open an incident.");

            _log.WriteErrorLog(msg.ToString(), ex);
            ExceptionEmail.Send(ex, msg.ToString());
        }

        private void HandleSocketResponseException(Exception ex, bool historyConnection)
        {
            if (((historyConnection && _successfulHistoryConnection) || (!historyConnection && _successfulOrderConnection)) && _unsentCount == 5)
            {
                if (historyConnection)
                {
                    _successfulHistoryConnection = false;
                }
                else
                {
                    _successfulOrderConnection = false;
                }


                StringBuilder msg = new StringBuilder();
                msg.AppendLine();
                msg.AppendLine();
                msg.AppendLine("******************************************************************************");
                msg.AppendLine("        Timed out waiting for response from mainframe after 5 reties.");
                msg.AppendLine("******************************************************************************");
                msg.AppendLine();
                msg.AppendLine();

                _log.WriteErrorLog(msg.ToString());
                ExceptionEmail.Send(ex, msg.ToString());
            }
            else
            {
                _unsentCount++;
            }
        }

        private void InitializeConfirmationThread()
        {
            _confirmationThread = new Thread(ProcessConfirmations);
            _confirmationThread.Start();
        }

        private void InitializeHistoryRequestTimer()
        {
            TimerCallback cb = new TimerCallback(ProcessOrderHistoryRequestsTick);
            AutoResetEvent auto = new AutoResetEvent(false);

            _historyRequestTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void InitializeHistoryResponseThread()
        {
            _historyResponseThread = new Thread(ProcessOrderHistoryListener);
            _historyResponseThread.Start();
        }

        private void InitializeOrderUpdateTimer()
        {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessOrderUpdatesTick);

            _orderUpdateTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void InitializeQueueTimer()
        {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessQueueTick);

            _queueTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }
        private void InitializeOrderHistoryCreatedWriter()
        {
            _orderHistoryWriterScope = _diContainer.BeginLifetimeScope();
            _orderHistoryWriter = _orderHistoryWriterScope.Resolve<IOrderHistoryWriter>();
            _orderHistoryWriter.ListenForNotificationMessagesOnQueue();
        }


        protected override void OnStart(string[] args)
        {
            _log.WriteInformationLog("Service starting");

            InitializeConfirmationThread();
            InitializeHistoryRequestTimer();
            InitializeHistoryResponseThread();
            InitializeOrderUpdateTimer();
            InitializeQueueTimer();
            InitializeOrderHistoryCreatedWriter();
        }

        protected override void OnStop()
        {
            TerminateHistoryRequestTimer();
            TerminateOrderUpdateTimer();
            TerminateQueueTimer();
            TerminateOrderHistoryCreatedWriter();
            _log.WriteInformationLog("Service stopping");
        }

        private void ProcessConfirmations()
        {
            try
            {

                using (_confirmationScope = _diContainer.BeginLifetimeScope())
                {
                    IConfirmationLogic confirmationLogic = _confirmationScope.Resolve<IConfirmationLogic>();

                    confirmationLogic.ListenForMainFrameCalls();
                }
            }
            catch (Exception e)
            {
                string logMessage = "Processing failed receiving confirmation. Processing of confirmations will not continue. Please restart this service.";

                _log.WriteErrorLog(logMessage);

                ExceptionEmail.Send(e, logMessage);
            }
        }

        private void ProcessOrderHistoryListener()
        {
            try
            {
                using (_historyResponseScope = _diContainer.BeginLifetimeScope())
                {
                    IOrderHistoryLogic logic = _historyResponseScope.Resolve<IOrderHistoryLogic>();

                    logic.ListenForMainFrameCalls();
                }
            }
            catch (Exception e)
            {
                string logMessage = "Processing failed receiving order updates. Processing of order updates will not continue. Please restart this service.";

                _log.WriteErrorLog(logMessage);

                ExceptionEmail.Send(e, logMessage);
            }
        }

        private void ProcessOrderHistoryRequestsTick(object state)
        {
            if (!_historyRequestProcessing)
            {
                _historyRequestProcessing = true;

                try
                {
                    if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
                    {
                        while (DateTime.Now.Hour < 5)
                        {
                            System.Threading.Thread.Sleep(60000);
                        }
                    }

                    using (_historyRequestScope = _diContainer.BeginLifetimeScope())
                    {
                        IOrderHistoryRequestLogic requestLogic = _historyRequestScope.Resolve<IOrderHistoryRequestLogic>();

                        requestLogic.ProcessRequests();
                    }

                    _successfulHistoryConnection = true;
                    _unsentCount = 0;
                }
                catch (EarlySocketException earlyEx)
                {
                    HandleEarlySocketException(earlyEx, true);
                }
                catch (SocketResponseException responseEx)
                {
                    HandleSocketResponseException(responseEx, true);
                }
                catch (CancelledTransactionException cancelledEx)
                {
                    HandleCancelledException(cancelledEx);
                }
                catch (Exception ex)
                {
                    _log.WriteErrorLog("Error processing order update requests", ex);
                    ExceptionEmail.Send(ex);
                }

                _historyRequestProcessing = false;
            }
        }

        public void ProcessOrderUpdatesTick(object state)
        {
            if (!_orderUpdateProcessing)
            {
                _orderUpdateProcessing = true;

                try
                {

                    // if update path does not exist, send an email then suppress messages
                    // disallow processing here until the folder exists, but do not kill the service
                    // to keep other threads running
                    if (Directory.Exists(Configuration.OrderUpdateWatchPath))
                    {
                        _allowOrderUpdateProcessing = true;
                        _silenceOrderUpdateMessages = false;
                    }
                    else
                    {
                        _allowOrderUpdateProcessing = false;

                        if (!_silenceOrderUpdateMessages)
                        {
                            HandleMissingOrderUpdateWatchPath();

                            _silenceOrderUpdateMessages = true;
                        }
                    }

                    if (_allowOrderUpdateProcessing)
                    {
                        string[] files = Directory.GetFiles(Configuration.OrderUpdateWatchPath);
                        _orderScope = _diContainer.BeginLifetimeScope();

                        IGenericQueueRepository repo = _orderScope.Resolve<IGenericQueueRepository>();

                        Parallel.ForEach(files, filePath =>
                        {
                            IOrderHistoryLogic logic = _orderScope.Resolve<IOrderHistoryLogic>();

                            if (CanOpenFile(filePath))
                            {
                                var logMessage = "Processing " + filePath + ".";
                                _log.WriteInformationLog(logMessage);

                                OrderHistoryFileReturn parsedFile = null;
                                using (var reader = File.OpenText(filePath))
                                {
                                    parsedFile = logic.ParseMainframeFile(reader);
                                }

                                if (parsedFile.Files.Count == 0)
                                {
                                    logMessage = "No instances of OrderHistoryFile were extracted from " + filePath + ".";
                                    _log.WriteWarningLog(logMessage);
                                }

                                var serializedFiles = new BlockingCollection<string>();

                                Parallel.ForEach(parsedFile.Files, file =>
                                {
                                    // do not upload an order file with an invalid header
                                    if (file.ValidHeader)
                                    {
                                        file.SenderApplicationName = Configuration.ApplicationName;
                                        file.SenderProcessName = "Process Order History Updates From Mainframe (Flat File)";

                                        try
                                        {
                                            var serializedFile = JsonConvert.SerializeObject(file);
                                            serializedFiles.Add(serializedFile);

                                            StringBuilder logMsg = new StringBuilder();
                                            logMsg.AppendLine(string.Format("Publishing order history to queue for message ({0}).", file.MessageId));
                                            logMsg.AppendLine();
                                            logMsg.AppendLine(serializedFile);

                                            _log.WriteInformationLog(logMsg.ToString());

                                            _silenceOrderUpdateMessages = false;
                                        }
                                        catch (Exception ex)
                                        {
                                            if (!_silenceOrderUpdateMessages)
                                            {
                                                HandleException(ex);
                                                _silenceOrderUpdateMessages = true;

                                            }
                                        }
                                    }
                                });

                                if (serializedFiles.Count > 0)
                                    repo.BulkPublishToQueue(serializedFiles.ToList(), Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeHourlyUpdates);

                                File.Delete(filePath);

                                logMessage = "Deleted " + filePath + ".";
                                _log.WriteInformationLog(logMessage);
                            } // end if CanOpenFile
                        });


                        _orderScope.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }

                _orderUpdateProcessing = false;
            }
        }

        private void ProcessQueueTick(object state)
        {
            if (!_orderQueueProcessing)
            {
                _orderQueueProcessing = true;

                // do not process between 1 and 5
                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
                {
                    _log.WriteInformationLog("Script stopped for processing window");

                    while (DateTime.Now.Hour < 5)
                    {
                        Thread.Sleep(60000);
                    }

                    _log.WriteInformationLog("Script started after processing window");
                }

                try
                {
                    using (_queueScope = _diContainer.BeginLifetimeScope())
                    {
                        IOrderQueueLogic orderQueue = _queueScope.Resolve<IOrderQueueLogic>();

                        orderQueue.ProcessOrders();
                    }

                    _successfulOrderConnection = true;
                    _unsentCount = 0;
                }
                catch (EarlySocketException earlyEx)
                {
                    HandleEarlySocketException(earlyEx, false);
                }
                catch (SocketResponseException responseEx)
                {
                    HandleSocketResponseException(responseEx, false);
                }
                catch (CancelledTransactionException cancelledEx)
                {
                    HandleCancelledException(cancelledEx);
                }
                catch (Exception ex)
                {
                    _log.WriteErrorLog("Error processing orders", ex);
                    ExceptionEmail.Send(ex, "", "Error processing orders");
                }

                _orderQueueProcessing = false;
            }
        }

        private void TerminateHistoryRequestTimer()
        {
            if (_historyRequestTimer != null)
            {
                _historyRequestTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
                _historyRequestTimer.Dispose();
            }

            if (_historyRequestScope != null)
            {
                _historyRequestScope.Dispose();
            }
        }

        private void TerminateOrderUpdateTimer()
        {
            if (_orderUpdateTimer != null)
            {
                _orderUpdateTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }

            if (_orderScope != null)
            {
                _orderScope.Dispose();
            }
        }

        private void TerminateQueueTimer()
        {
            if (_queueTimer != null)
            {
                _queueTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }

            if (_queueScope != null)
            {
                _queueScope.Dispose();
            }
        }

        private void TerminateOrderHistoryCreatedWriter()
        {
            if (_orderHistoryWriter != null)
                _orderHistoryWriter.Stop();

            if (_orderHistoryWriterScope != null)
                _orderHistoryWriterScope.Dispose();
        }

        #endregion
    }
}
