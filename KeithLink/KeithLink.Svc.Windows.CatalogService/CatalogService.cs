using KeithLink.Common.Core.Logging;

using Autofac;

using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using System.Net;
using System.IO;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using KeithLink.Svc.Core.Interface.ETL;
using System.Data;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Windows.CatalogService.Helpers;
using System.Threading;
using KeithLink.Svc.Impl;

namespace KeithLink.Svc.Windows.CatalogService
{
    partial class CatalogService : ServiceBase
    {
        private IContainer container;
        private IEventLogRepository _log;
        private Task processImagesTask;
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
                (processTime.IndexOf(':') + 1)))
            {
                processImagesTask = Task.Factory.StartNew(() => UnfiImageProcessing.StartProcessAllImages(_log));
            }

            //compensate for delay in processing to keep at top of minute
            int frac = 1000 - DateTime.Now.Millisecond;
            int sec = 59 - DateTime.Now.Second;
            _timer.Change((sec * 1000) + frac, TIMER_DURATION_TICKMINUTE);
        }
    }
}
