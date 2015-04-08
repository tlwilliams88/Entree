using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Logic.SingleSignOn;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.SingleSignOn;
using KeithLink.Svc.Impl.Logic.PowerMenu;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace KeithLink.Svc.Windows.AccessService {
    public partial class AccessService : ServiceBase {
        #region attributes
        private KbitRequestLogicImpl _kbitLogic;
        private PowerMenuLogicImpl _pmLogic;
        private EventLogRepositoryImpl _log;
        private AccessRequestLogicImpl _requestLogic;
        #endregion

        #region ctor
        public AccessService() {
            InitializeComponent();

            _kbitLogic = new KbitRequestLogicImpl(new KbitRepositoryImpl());
            _pmLogic = new PowerMenuLogicImpl( new KeithLink.Svc.Impl.Repository.PowerMenu.PowerMenuRepositoryImpl( _log ) );
            _log = new EventLogRepositoryImpl(this.ServiceName);
            _requestLogic = new AccessRequestLogicImpl(new GenericQueueRepositoryImpl(), _kbitLogic, _log, _pmLogic);
        }
        #endregion

        #region methods
        protected override void OnStart(string[] args) {
            _log.WriteInformationLog("Service starting");

            _requestLogic.ProcessQueuedRequests();
        }

        protected override void OnStop() {
            _log.WriteInformationLog("Service stopped");

            _requestLogic.StopProcessing();
        }
        #endregion
    }
}
