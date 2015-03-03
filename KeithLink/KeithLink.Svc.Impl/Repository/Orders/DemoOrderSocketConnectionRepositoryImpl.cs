using KeithLink.Svc.Core.Interface.Common;
using System;

namespace KeithLink.Svc.Impl.Repository.Orders {
    public class DemoOrderSocketConnectionRepositoryImpl : ISocketConnectionRepository {
        #region methods
        public void Connect() {
        }

        public void Close() {
        }

        public string Receive() {
            return string.Empty;
        }

        public void Send(string dataRecord) {
        }

        public void StartTransaction(string confirmationNumber) {
        }
        #endregion
    }
}
