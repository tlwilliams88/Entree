using KeithLink.Common.Core.Email;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using KeithLink.Svc.Impl.Repository.Queue;
using Newtonsoft.Json;
using KeithLink.Svc.Impl.Repository.Cache;

namespace KeithLink.Svc.Windows.OrderService {
    partial class OrderService : ServiceBase {

        #region attributes
        private EventLogRepositoryImpl _log;

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
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public OrderService() {
            InitializeComponent();

            _log = new EventLogRepositoryImpl(this.ServiceName);
            _orderQueueProcessing = false;
            _successfulHistoryConnection = false;
            _successfulOrderConnection = false;
            _unsentCount = 0;
            _orderUpdateProcessing = false;

            _allowOrderUpdateProcessing = true;
            _silenceOrderUpdateMessages = false;
        }
        #endregion

        #region methods
        private bool CanOpenFile(string filePath) {
            bool opened = false;
            int loopCnt = 0;

            while (loopCnt < 30 && !opened) {
                try {
                    System.IO.FileStream myFile = System.IO.File.OpenWrite(filePath);
                    myFile.Close();
                    myFile.Dispose();

                    opened = true;
                } catch {
                    System.Threading.Thread.Sleep(1000);
                    loopCnt++;
                }
            }

            return opened;
        }

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

        private void HandleEarlySocketException(Exception ex, bool historyConnection) {
            if ((historyConnection && _successfulHistoryConnection) || (!historyConnection && _successfulOrderConnection)) {
                if (historyConnection) {
                    _successfulHistoryConnection = false;
                } else {
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

        private void HandleSocketResponseException(Exception ex, bool historyConnection) {
            if (((historyConnection && _successfulHistoryConnection) || (!historyConnection  && _successfulOrderConnection)) && _unsentCount == 5) {
                if (historyConnection) {
                    _successfulHistoryConnection = false;
                } else {
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
                KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, msg.ToString());
            } else {
                _unsentCount++;
            }
        }

        private void InitializeConfirmationThread()
        {
            _confirmationThread = new Thread(ProcessConfirmations);
            _confirmationThread.Start();
        }

        private void InitializeHistoryRequestTimer() {
            TimerCallback cb = new TimerCallback(ProcessOrderHistoryRequestsTick);
            AutoResetEvent auto = new AutoResetEvent(false);

            _historyRequestTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void InitializeHistoryResponseThread() {
            _historyResponseThread = new Thread(ProcessOrderHistoryListener);
            _historyResponseThread.Start();
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

        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

			InitializeConfirmationThread();
			InitializeHistoryRequestTimer();
			InitializeHistoryResponseThread();
			InitializeOrderUpdateTimer();
			InitializeQueueTimer();
        }

        protected override void OnStop() {
            TerminateHistoryRequestTimer();
            TerminateOrderUpdateTimer();
            TerminateQueueTimer();

            _log.WriteInformationLog("Service stopping");
        }

        private void ProcessConfirmations() {
            try {
				UnitOfWork uow = new UnitOfWork(_log);
                OrderConversionLogicImpl conversionLogic = new OrderConversionLogicImpl(new OrderHistoyrHeaderRepositoryImpl(uow), uow, _log);

                ConfirmationLogicImpl confirmationLogic = new ConfirmationLogicImpl(_log,
                                                                       new KeithLink.Svc.Impl.Repository.Network.SocketListenerRepositoryImpl(),
                                                                       new KeithLink.Svc.Impl.Repository.Queue.GenericQueueRepositoryImpl(),
                                                                       conversionLogic,
                                                                       uow);
                confirmationLogic.ListenForMainFrameCalls();
            }
            catch (Exception e) {
                string logMessage = "Processing failed receiving confirmation. Processing of confirmations will not continue. Please restart this service.";
                
                _log.WriteErrorLog(logMessage);

                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, logMessage);
            }
        }

        private void ProcessOrderHistoryListener() {
            try {
				UnitOfWork uow = new UnitOfWork(_log);
                OrderConversionLogicImpl conversionLogic = new OrderConversionLogicImpl(new OrderHistoyrHeaderRepositoryImpl(uow), uow, _log);

                KeithLink.Svc.Impl.Repository.SiteCatalog.DivisionRepositoryImpl divRepo = new KeithLink.Svc.Impl.Repository.SiteCatalog.DivisionRepositoryImpl();

                KeithLink.Svc.Impl.Logic.SiteCatalog.SiteCatalogLogicImpl catLogic =
                new KeithLink.Svc.Impl.Logic.SiteCatalog.SiteCatalogLogicImpl(new KeithLink.Svc.Impl.Repository.SiteCatalog.ElasticSearchCatalogRepositoryImpl(),
                                                                              new KeithLink.Svc.Impl.Logic.PriceLogicImpl(new KeithLink.Svc.Impl.Repository.SiteCatalog.PriceRepositoryImpl(),
																														  new NoCacheRepositoryImpl()),
                                                                              new KeithLink.Svc.Impl.Repository.SiteCatalog.ProductImageRepositoryImpl(),
                                                                              new KeithLink.Svc.Impl.Repository.Lists.NoListServiceRepositoryImpl(),
                                                                              divRepo,
                                                                              new KeithLink.Svc.Impl.Repository.SiteCatalog.CategoryImageRepository(_log),
																			  new NoCacheRepositoryImpl(),
                                                                              new KeithLink.Svc.Impl.Logic.DivisionLogicImpl(divRepo,
                                                                                                                             new KeithLink.Svc.Impl.Repository.SiteCatalog.NoDivisionServiceRepositoryImpl()),
                                                                              new KeithLink.Svc.Impl.Repository.Orders.NoOrderServiceRepositoryImpl());

                OrderHistoryLogicImpl logic = new OrderHistoryLogicImpl(_log,
                                                                       new GenericQueueRepositoryImpl(),
                                                                       new KeithLink.Svc.Impl.Repository.Network.SocketListenerRepositoryImpl());

                logic.ListenForMainFrameCalls();
            } catch (Exception e) {
                string logMessage = "Processing failed receiving order updates. Processing of order updates will not continue. Please restart this service.";

                _log.WriteErrorLog(logMessage);

                KeithLink.Common.Core.Email.ExceptionEmail.Send(e, logMessage);
            }
        }

        private void ProcessOrderHistoryRequestsTick(object state) {
            if (!_historyRequestProcessing) {
                _historyRequestProcessing = true;

                try {
                    if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5) {
                        while (DateTime.Now.Hour < 5) {
                            System.Threading.Thread.Sleep(60000);
                        }
                    }

					OrderHistoryRequestLogicImpl requestLogic = new OrderHistoryRequestLogicImpl(_log, new GenericQueueRepositoryImpl(), new OrderUpdateRequestSocketRepositoryImpl());

                    requestLogic.ProcessRequests();

                    _successfulHistoryConnection = true;
                    _unsentCount = 0;
                } catch (EarlySocketException earlyEx) {
                    HandleEarlySocketException(earlyEx, true);
                } catch (SocketResponseException responseEx) {
                    HandleSocketResponseException(responseEx, true);
                } catch (CancelledTransactionException cancelledEx) {
                    HandleCancelledException(cancelledEx);
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error processing order update requests", ex);
                    KeithLink.Common.Core.Email.ExceptionEmail.Send(ex);
                }

                _historyRequestProcessing = false;
            }
        }

        private void ProcessOrderUpdatesTick(object state) {
            if (!_orderUpdateProcessing) {
                _orderUpdateProcessing = true;

                try {

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
						GenericQueueRepositoryImpl repo = new GenericQueueRepositoryImpl();

						var items = new System.Collections.Concurrent.BlockingCollection<string>();
				
						System.Threading.Tasks.Parallel.ForEach(files, filePath =>
						{
							OrderHistoryLogicImpl logic = new OrderHistoryLogicImpl(_log,
																				   new GenericQueueRepositoryImpl(),
																				   new KeithLink.Svc.Impl.Repository.Network.SocketListenerRepositoryImpl());

							if (CanOpenFile(filePath))
							{
								OrderHistoryFileReturn parsedFile = logic.ParseMainframeFile(filePath);
								System.Threading.Tasks.Parallel.ForEach(parsedFile.Files, file =>
								{

									// do not upload an order file with an invalid header
									if (file.ValidHeader)
									{
										file.SenderApplicationName = Configuration.ApplicationName;
										file.SenderProcessName = "Process Order History Updates From Mainframe (Flat File)";

										try
										{
											var jsonValue = JsonConvert.SerializeObject(file);
											items.Add(jsonValue);
											StringBuilder logMsg = new StringBuilder();
											logMsg.AppendLine(string.Format("Publishing order history to queue for message ({0}).", file.MessageId));
											logMsg.AppendLine();
											logMsg.AppendLine(jsonValue);

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

								if (items.Count > 0)
									repo.BulkPublishToQueue(items.ToList(), Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQExchangeHourlyUpdates);
						
								System.IO.File.Delete(filePath);
							} // end if CanOpenFile
						});

											

                    }
                } catch (Exception ex) {
                    HandleException(ex);
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
                                                                   new GenericQueueRepositoryImpl(),
                                                                   new KeithLink.Svc.Impl.Repository.Orders.OrderSocketConnectionRepositoryImpl());
                    orderQueue.ProcessOrders();

                    _successfulOrderConnection = true;
                    _unsentCount = 0;
                } catch(EarlySocketException earlyEx){
                    HandleEarlySocketException(earlyEx, false);
                } catch(SocketResponseException responseEx){
                    HandleSocketResponseException(responseEx, false);
                } catch(CancelledTransactionException cancelledEx){
                    HandleCancelledException(cancelledEx);
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error processing orders", ex);
                    KeithLink.Common.Core.Email.ExceptionEmail.Send(ex);
                }

                _orderQueueProcessing = false;
            }
        }

        private void TerminateHistoryRequestTimer() {
            if (_historyRequestTimer != null) {
                _historyRequestTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
                _historyRequestTimer.Dispose();
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
