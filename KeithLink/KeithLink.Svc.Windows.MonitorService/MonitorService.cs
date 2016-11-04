using Autofac;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.ApplicationHealth;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.MonitorService
{
    public partial class MonitorService : ServiceBase
    {
        #region attributes
        private IOrderHistoryLogic _orderHistoryLogic;
        private IEventLogRepository _log;
        private IEmailClient _emailClient;

        private Autofac.IContainer container;

        private ILifetimeScope checkQueueSizesScope;
        private ILifetimeScope checkLostOrdersScope;

        private Timer _generalTimer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        public MonitorService()
        {
            container = DependencyMapFactory.GetMonitorSvcContainer
                            (Core.Enumerations.Dependencies.DependencyInstanceType.InstancePerLifetimeScope).Build();
            InitializeComponent();
        }

        #region methods
        protected override void OnStart(string[] args)
        {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog("Service starting");

            InitializeGeneralTimer();
        }

        protected override void OnStop()
        {
            _log.WriteInformationLog("Service stopping");

            TerminateGeneralTimer();

            _log.WriteInformationLog("Service stopped");

        }
        private void InitializeGeneralTimer()
        {
            AutoResetEvent auto = new AutoResetEvent(false);
            TimerCallback cb = new TimerCallback(ProcessMinuteTick);
            _generalTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICKMINUTE);
        }

        private void TerminateGeneralTimer()
        {
            _generalTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
        }

        private void ProcessMinuteTick(object state)
        {
            // end processing minute between 1 and 5
            if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
            {
                return;
            }

            // only process at the top of the hour
            if (DateTime.Now.Minute == 0 && DoCheckLostOrders())
            //if (true) // testing only
            {
                ProcessCheckLostOrders();
            }

            // process every minute
            if (DoCheckQueueSizes())
            //if (true) // testing only
            {
                ProcessCheckQueueSizes();
            }
        }

        private bool DoCheckQueueSizes()
        {
            string checksetting = Configuration.CheckQueueSizes;
            List<string> excluded = new List<string>();
            if (checksetting.IndexOf('|') > -1)
            {
                string exclusions = checksetting.Substring(checksetting.IndexOf('|') + 1);
                checksetting = checksetting.Substring(0, checksetting.IndexOf('|'));
                excluded = Configuration.GetCommaSeparatedValues(exclusions.ToUpper());
            }

            if (checksetting.Equals("true", StringComparison.CurrentCultureIgnoreCase) &&
                excluded.Contains(System.Environment.MachineName) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ProcessCheckQueueSizes()
        {
            try
            {
                checkQueueSizesScope = container.BeginLifetimeScope();
                IQueueHealthCheck healthCheck = checkQueueSizesScope.Resolve<IQueueHealthCheck>();

                healthCheck.CheckQueueProperties();
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error in ProcessCheckQueueSizes", ex);
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex);
            }
        }

        private bool DoCheckLostOrders()
        {
            string checksetting = Configuration.CheckLostOrders;
            List<string> excluded = new List<string>();
            if (checksetting.IndexOf('|') > -1)
            {
                string exclusions = checksetting.Substring(checksetting.IndexOf('|') + 1);
                checksetting = checksetting.Substring(0, checksetting.IndexOf('|'));
                excluded = Configuration.GetCommaSeparatedValues(exclusions.ToUpper());
            }

            if (checksetting.Equals("true", StringComparison.CurrentCultureIgnoreCase) &&
                excluded.Contains(System.Environment.MachineName) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ProcessCheckLostOrders()
        {
            try
            {
                string subject;
                string body;

                checkLostOrdersScope = container.BeginLifetimeScope();
                _orderHistoryLogic = checkLostOrdersScope.Resolve<IOrderHistoryLogic>();
                _emailClient = checkLostOrdersScope.Resolve<IEmailClient>();

                subject = _orderHistoryLogic.CheckForLostOrders(out body);

                StringBuilder sbMsgBody = new StringBuilder();
                sbMsgBody.Append(body);

                if ((subject != null) && (subject.Length > 0) && (body != null) && (body.Length > 0))
                {
                    //_log.WriteErrorLog("BEK: " + subject + ";" + body );
                    _emailClient.SendEmail(Configuration.FailureEmailAdresses, null, null, "BEK: " + subject, body);
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error in ProcessCheckLostOrders", ex);
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex);
            }
        }
        #endregion
    }
}
