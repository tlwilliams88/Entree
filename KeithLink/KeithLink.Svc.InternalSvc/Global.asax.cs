using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Confirmations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KeithLink.Svc.InternalSvc
{
    public class Global : System.Web.HttpApplication {
        #region attributes
        const int TIMER_DURATION_TICK = 2000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        
        private bool _keepQueueListening;
        private bool _orderHistoryProcessing;

        private int _instanceCount;

        private Thread _orderHistoryThread;
        private Timer _orderHistoryTimer;
        #endregion

        #region ctor
        public Global() {
            _keepQueueListening = true;
            _orderHistoryProcessing = false;
        }
        #endregion

        #region events
        protected void Application_Start(object sender, EventArgs e)
        {
            IContainer container = AutofacContainerBuilder.BuildContainer();
            AutofacHostFactory.Container = container;

            //InitializeOrderUpdateTimer();
            InitializeOrderUpdateThread();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            _keepQueueListening = false;

            TerminateOrderHistoryTimer();
        }
        #endregion

        #region methods
        private void InitializeOrderUpdateThread() {
            System.Diagnostics.Debugger.Launch();

            ThreadStart start = new ThreadStart(OrderHistoryQueueListener);
            _orderHistoryThread = new Thread(start);

            _orderHistoryThread.Start();
        }

        private void InitializeOrderUpdateTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            //TimerCallback cb = new TimerCallback(OrderHistoryQueueListener);

            System.Diagnostics.Debugger.Launch();

            //_orderHistoryTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void OrderHistoryQueueListener() 
        {
            while (_keepQueueListening)
            {
                if (_orderHistoryProcessing == false) {
                    _orderHistoryProcessing = true;
                    _instanceCount++;

                    try {
                        IOrderHistoryQueueRepository historyQueue = ((IContainer)AutofacHostFactory.Container).Resolve<IOrderHistoryQueueRepository>();
                        IOrderHistoryLogic historyLogic = ((IContainer)AutofacHostFactory.Container).Resolve<IOrderHistoryLogic>();

                        string rawOrder = historyQueue.ConsumeFromQueue();

                        while (rawOrder != null) {
                            OrderHistoryFile historyFile = new OrderHistoryFile();

                            System.IO.StringReader xmlData = new System.IO.StringReader(rawOrder);
                            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(historyFile.GetType());

                            historyFile = (OrderHistoryFile)xs.Deserialize(xmlData);

                            historyLogic.Save(historyFile);

                            rawOrder = historyQueue.ConsumeFromQueue();

                        }
                    } catch (Exception ex) {
                        IEventLogRepository eventLogRepository = ((IContainer)AutofacHostFactory.Container).Resolve<IEventLogRepository>();
                        eventLogRepository.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                    }

                    _orderHistoryProcessing = false;
                    _instanceCount--;
                }

                System.Threading.Thread.Sleep(2000);
            }
        }

        private void TerminateOrderHistoryTimer() {
            if (_orderHistoryTimer != null) {
                _orderHistoryTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
                _orderHistoryTimer.Dispose();
            }
        }
        #endregion
    }
}