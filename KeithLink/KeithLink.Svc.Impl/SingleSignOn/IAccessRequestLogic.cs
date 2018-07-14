using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.SingleSignOn {
    public interface IAccessRequestLogic {
        void ProcessQueuedRequests();

        void StopProcessing();
    }
}
