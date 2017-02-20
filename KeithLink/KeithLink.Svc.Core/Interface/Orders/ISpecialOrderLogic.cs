using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface ISpecialOrderLogic {
        void ListenForQueueMessages();

        void StopListening();

        void SubscribeToQueue();

        void Unsubscribe();
    }
}
