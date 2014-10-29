using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
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
        
        private bool _keepQueueListening;

        private Thread _orderHistoryThread;
        #endregion

        #region ctor
        public Global() {
            _keepQueueListening = true;
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

        private void OrderHistoryQueueListener() 
        {
            while (_keepQueueListening)
            {
                
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

                System.Threading.Thread.Sleep(TIMER_DURATION_TICK);
            }
        }

        #endregion
    }
}