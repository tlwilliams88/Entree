using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Logic.Orders;
using System;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace KeithLink.Svc.Windows.OrderService {
    partial class OrderService : ServiceBase {

        #region attributes
        private EventLogRepositoryImpl _log;
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
        }
        #endregion

        #region methods
        private void InitializeQueueTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(ProcessQueueTick);

            _queueTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

            InitializeQueueTimer();
        }

        protected override void OnStop() {
            TerminateQueueTimer();

            _log.WriteInformationLog("Service stopping");
        }

        private void ProcessQueueTick(object state) {
            try {
                OrderLogicImpl orderQueue = new OrderLogicImpl(_log,
                                                               new KeithLink.Svc.Impl.Repository.Orders.OrderQueueRepositoryImpl(),
                                                               new KeithLink.Svc.Impl.Repository.Orders.OrderSocketConnectionRepositoryImpl());

                orderQueue.ProcessOrders();
            } catch (Exception ex) {
                _log.WriteErrorLog("Error processing orders", ex);
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
