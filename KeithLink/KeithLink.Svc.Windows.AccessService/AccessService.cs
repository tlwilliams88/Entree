using Autofac;
using KeithLink.Svc.Impl.Logic.SingleSignOn;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.SingleSignOn;
using KeithLink.Svc.Impl.Logic.PowerMenu;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.SingleSignOn;
using KeithLink.Svc.Core.Interface.PowerMenu;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace KeithLink.Svc.Windows.AccessService {
    public partial class AccessService : ServiceBase {
        #region attributes
        private IContainer _container;

        private IEventLogRepository _log;
        private IAccessRequestLogic _requestLogic;
        #endregion

        #region ctor
        public AccessService() {
            _container = Impl.Repository.SmartResolver.DependencyMapFactory.GetAccessServiceContainer().Build();

            _log = _container.Resolve<IEventLogRepository>();
            //_requestLogic = container.Resolve<IAccessRequestLogic>();

            InitializeComponent();

            //_kbitLogic = new KbitRequestLogicImpl();
            //_pmLogic = new PowerMenuLogicImpl();
            //_log = new EventLogRepositoryImpl(this.ServiceName);
            //_requestLogic = new AccessRequestLogicImpl(new GenericQueueRepositoryImpl(), _kbitLogic, _log, _pmLogic);
        }
        #endregion

        #region methods
        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

            ILifetimeScope accessQueueScope = _container.BeginLifetimeScope();

            _requestLogic =  accessQueueScope.Resolve<IAccessRequestLogic>();
            _requestLogic.ProcessQueuedRequests();
        }

        protected override void OnStop() {
            _log.WriteInformationLog("Service stopped");

            _requestLogic.StopProcessing();
        }
        #endregion
    }
}
