using Autofac;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic.IxOne;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Threading;

namespace KeithLink.Svc.Windows.CatalogService
{
    partial class CatalogService : ServiceBase
    {
        private IContainer container;
        private IEventLogRepository _log;
        private Task processImagesTask;
        private bool processImagesRunning;
        private Timer _timer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;

        public CatalogService()
        {
            container = DependencyMapFactory.GetCatalogSvcContainer().Build();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog("Service starting");

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            int frac = 1000 - DateTime.Now.Millisecond;
            int sec = 59 - DateTime.Now.Second;
            AutoResetEvent auto = new AutoResetEvent(false);
            TimerCallback cb = new TimerCallback(ProcessMinuteTick);
            _timer = new Timer(cb, auto, (sec * 1000) + frac, TIMER_DURATION_TICKMINUTE);
        }

        protected override void OnStop()
        {
            _log.WriteInformationLog("Service stopped");
        }

        private void TerminateTimer()
        {
            if (_timer != null)
            {
                _timer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }
        }

        private void ProcessMinuteTick(object state)
        {
            // only process at time set in appSettings
            string processTime = Configuration.CatalogServiceUnfiImagesProcessTime;
            if (DateTime.Now.Hour == int.Parse(processTime.Substring
                (0, processTime.IndexOf(':'))) &&
                DateTime.Now.Minute == int.Parse(processTime.Substring
                (processTime.IndexOf(':') + 1)) &&
                processImagesRunning == false)
            {
                processImagesRunning = true;
                processImagesTask = Task.Factory.StartNew(() => UnfiImageProcessing.StartProcessAllImages(_log));
                processImagesRunning = false;
            }

            //compensate for delay in processing to keep at top of minute
            int frac = 1000 - DateTime.Now.Millisecond;
            int sec = 59 - DateTime.Now.Second;
            _timer.Change((sec * 1000) + frac, TIMER_DURATION_TICKMINUTE);
        }
    }
}
