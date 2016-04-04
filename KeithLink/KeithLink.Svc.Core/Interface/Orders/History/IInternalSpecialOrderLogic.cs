using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders.History
{
    public interface IInternalSpecialOrderLogic
    {
        void ListenForQueueMessages();

        void StopListening();

    }
}
