using KeithLink.Svc.Core.Interface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Queue {
    public class DemoQueueRepositoryImpl : IGenericQueueRepository {
        #region methods
        public string ConsumeFromQueue(string serverName, string userName, string password, string virtualHost, string queue) {
            //throw new NotImplementedException();

            //todo add logic for each different queue(e.g. order history)

            return string.Empty;
        }

        public void PublishToQueue(string item, string serverName, string userName, string password, string virtualHost, string exchange) {
            // do nothing
        }
        #endregion
    }
}
