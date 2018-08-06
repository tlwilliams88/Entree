using System;
using System.Net;
using System.Net.Sockets;
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

        void BindSocketToPort(Socket socket, int port);
        void Listen(int port);
    }
}
