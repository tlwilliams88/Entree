using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Events.EventHandlers;

namespace KeithLink.Svc.Core.Interface.Common
{
    public interface ISocketListenerRepository
    {
        event EventHandler BeginningFileReceipt;
        event EventHandler ClosedPort;
        event ExceptionEventHandler ErrorEncountered;
        event FileReceivedHandler FileReceived;
        event EventHandler OpeningPort;
        event EventHandler WaitingConnection;

        void Listen(int port);
    }
}
