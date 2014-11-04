using KeithLink.Svc.Core.Models.Orders.Confirmations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders.Confirmations
{
    public interface IConfirmationLogic
    {
        void ListenForMainFrameCalls();
        
        void ListenForQueueMessages();
        
        bool ProcessIncomingConfirmation(ConfirmationFile confirmation);
        
        void Stop();
    }
}
