using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Events.EventArgs;
using KeithLink.Svc.Core.Events.EventHandlers;
using KeithLink.Svc.Core.Interface.Common;

using System;
using KeithLink.Common.Core.Interfaces.Logging;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KeithLink.Svc.Impl.Repository.Network
{
    public class SocketListenerRepositoryImpl : ISocketListenerRepository, IDisposable
    {
        #region attributes
        private Socket _handler;
        private bool _disposed;
        private IEventLogRepository _log;

        public event EventHandler BeginningFileReceipt;
        public event EventHandler ClosedPort;
        public event ExceptionEventHandler ErrorEncountered;
        public event FileReceivedHandler FileReceived;
        public event EventHandler OpeningPort;
        public event EventHandler WaitingConnection;
        #endregion

        #region ctor
        public SocketListenerRepositoryImpl(IEventLogRepository eventLogRepository)
        {
            _log = eventLogRepository;
            _disposed = false;
            _handler = null;
        }
        #endregion

        #region methods
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_handler != null) { _handler.Dispose(); }
            }

            _disposed = true;
        }

        public void BindSocketToPort(Socket socket, int port)
        {
            int attempts = 0;

            while (socket.IsBound == false)
            {
                attempts++;

                if (attempts == 300)    // 5 minutes @ 1 attempt per second
                {
                    string errorMessage = string.Format("A socket listener has reached a limit of {0} attempts to obtain port {1}.", attempts, port);
                    _log.WriteErrorLog(errorMessage);

                    throw new ApplicationException(errorMessage);
                }

                if (attempts % 10 == 0)    // 1 minutes @ 1 attempt per second
                {
                    string warningMessage = string.Format("A socket listener has made {0} attempts to obtain port {1}.", attempts, port);
                    _log.WriteWarningLog(warningMessage);
                }

                try
                {
                    var endPoint = new IPEndPoint(IPAddress.Any, port);
                    socket.Bind(endPoint);
                }
                catch (SocketException ex)
                {
                    int WSAEADDRINUSE = 10048;

                    if (ex.ErrorCode == WSAEADDRINUSE)
                    {
                        Thread.Sleep(1000);  // allow port to be released
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void Listen(int port)
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                OnOpeningPort(new EventArgs());

                BindSocketToPort(listener, port);

                listener.Listen(10);

                while (true)
                {
                    OnWaitingConnection(new EventArgs());
                    _handler = listener.Accept();

                    OnBeginningFileReceipt(new EventArgs());
                    StringBuilder data = new StringBuilder();
                    bool receivingFile = true;

                    while (receivingFile)
                    {
                        Byte[] bytes = new Byte[1024];
                        int bytesReceived = _handler.Receive(bytes);

                        data.Append(Encoding.ASCII.GetString(bytes, 0, bytesReceived));

                        if (data.ToString().IndexOf("END###") > -1)
                        {
                            OnFileReceived(new ReceivedFileEventArgs(data.ToString()));

                            _handler.Send(Encoding.ASCII.GetBytes("OK"));

                            data = new StringBuilder();
                        }

                        if (data.ToString().IndexOf("BYE###") > -1)
                        {
                            receivingFile = false;
                        }
                    }

                    _handler.Shutdown(SocketShutdown.Both);
                    _handler.Close();
                }
            }
            catch (Exception e)
            {
                if (_handler != null && _handler.Connected)
                {
                    _handler.Shutdown(SocketShutdown.Both);
                    _handler.Close();
                }

                OnErrorEncountered(new ExceptionEventArgs(e));
            }

            OnClosedPort(new EventArgs());
        }

        protected virtual void OnBeginningFileReceipt(EventArgs e)
        {
            if (BeginningFileReceipt != null)
            {
                BeginningFileReceipt(this, e);
            }
        }

        protected virtual void OnClosedPort(EventArgs e)
        {
            if (ClosedPort != null)
            {
                ClosedPort(this, e);
            }
        }

        protected virtual void OnErrorEncountered(ExceptionEventArgs e)
        {
            if (ErrorEncountered != null)
            {
                ErrorEncountered(this, e);
            }
        }

        protected virtual void OnFileReceived(ReceivedFileEventArgs e)
        {
            if (FileReceived != null)
            {
                FileReceived(this, e);
            }
        }

        protected virtual void OnOpeningPort(EventArgs e)
        {
            if (OpeningPort != null)
            {
                OpeningPort(this, e);
            }
        }

        protected virtual void OnWaitingConnection(EventArgs e)
        {
            if (WaitingConnection != null)
            {
                WaitingConnection(this, e);
            }
        }
        #endregion
    }
}
