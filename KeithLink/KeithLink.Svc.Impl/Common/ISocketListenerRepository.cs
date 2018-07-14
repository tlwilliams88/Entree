using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Events.EventArgs;
using Entree.Core.Events.EventHandlers;

namespace Entree.Core.Interface.Common
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
