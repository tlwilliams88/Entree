using Autofac;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.ApplicationHealth;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.ApplicationHealth;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using System;
using System.Collections.Generic;
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
        private IContainer _container;
        private ILifetimeScope _lostOrdersScope;
        private ILifetimeScope _queueCheckScope;

        private bool _checkingLostOrders;
        private bool _checkingQueueProperties;

        private IOrderHistoryLogic _orderHistoryLogic;
        private IApplicationHealthLogic _appHealthLogic;
        private IEventLogRepository _log;
        private IEmailClient _emailClient;

        private Timer _generalTimer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion
        public MonitorService()
        {
            _container = DependencyMapFactory.GetMonitorSvcContainer
                (Core.Enumerations.Dependencies.DependencyInstanceType.InstancePerLifetimeScope).Build();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _log = _container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog("Service starting");
            InitializeGeneralTimer();
        }
        
        protected override void OnStop()
        {
            _log.WriteInformationLog("Service stopping");
            TerminateGeneralTimer();
        }

        private void InitializeGeneralTimer()
        {
            AutoResetEvent auto = new AutoResetEvent(false);
            TimerCallback cb = new TimerCallback(ProcessMinuteTick);
            _generalTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICKMINUTE);
        }

        private void TerminateGeneralTimer()
        {
            if (_generalTimer != null)
            {
                _generalTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }
        }

        private void ProcessMinuteTick(object state)
        {
            // process every minute
            if (_checkingQueueProperties == false &&
                DoCheckQueueHealth())
            {
                ProcessCheckQueueHealth();
            }

            // only process at the top of the hour when not during quiet time between 1 and 5
            if (_checkingLostOrders == false &&
                (DateTime.Now.Hour < 1 | 
                DateTime.Now.Hour > 5) && 
                DateTime.Now.Minute == 0 && 
                DoCheckLostOrders())
            {
                ProcessCheckLostOrders();
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
            _checkingLostOrders = true;

            try
            {
                string subject;
                string body;

                _lostOrdersScope = _container.BeginLifetimeScope();
                _orderHistoryLogic = _lostOrdersScope.Resolve<IOrderHistoryLogic>();
                //_log.WriteInformationLog("ProcessCheckLostOrders");
                subject = _orderHistoryLogic.CheckForLostOrders(out body);

                StringBuilder sbMsgBody = new StringBuilder();
                sbMsgBody.Append(body);

                if ((subject != null) && (subject.Length > 0) && (body != null) && (body.Length > 0))
                {
                    _emailClient = _lostOrdersScope.Resolve<IEmailClient>();
                    _emailClient.SendEmail(Configuration.FailureEmailAdresses, null, null, "BEK: " + subject, body);
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error in ProcessCheckLostOrders", ex);
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex);
            }

            _checkingLostOrders = false;
        }

        private bool DoCheckQueueHealth()
        {
            string checksetting = Configuration.CheckQueueHealth;
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

        private void ProcessCheckQueueHealth()
        {
            _checkingQueueProperties = true;

            try
            {
                _queueCheckScope = _container.BeginLifetimeScope();
                _appHealthLogic = _queueCheckScope.Resolve<IApplicationHealthLogic>();
                //_log.WriteInformationLog("ProcessCheckQueueHealth");
                _appHealthLogic.CheckQueueProperties();
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error in ProcessCheckQueueHealth", ex);
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex);
            }

            _checkingQueueProperties = false;
        }
    }
}
