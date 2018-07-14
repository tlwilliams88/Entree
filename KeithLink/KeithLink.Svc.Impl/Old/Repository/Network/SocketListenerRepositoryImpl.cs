﻿using Entree.Core;
using Entree.Core.Events.EventArgs;
using Entree.Core.Events.EventHandlers;
using Entree.Core.Exceptions.Orders;
using Entree.Core.Interface.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Network {
    public class SocketListenerRepositoryImpl : ISocketListenerRepository, IDisposable {
        #region attributes
        private Socket _handler;
        private bool _disposed;

        public event EventHandler BeginningFileReceipt;
        public event EventHandler ClosedPort;
        public event ExceptionEventHandler ErrorEncountered;
        public event FileReceivedHandler FileReceived;
        public event EventHandler OpeningPort;
        public event EventHandler WaitingConnection;
        #endregion

        #region ctor
        public SocketListenerRepositoryImpl() {
            _disposed = false;
            _handler = null;
        }
        #endregion

        #region methods
        public void Dispose() {
            if (!_disposed) {
                if (_handler != null) { _handler.Dispose(); }
            }

            _disposed = true;
        }

        public void Listen(int listeningPort) {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, listeningPort);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                OnOpeningPort(new EventArgs());

                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)  {
                    OnWaitingConnection(new EventArgs());
                    _handler = listener.Accept();

                    OnBeginningFileReceipt(new EventArgs());
                    StringBuilder data = new StringBuilder();
                    bool receivingFile = true;

                    while (receivingFile) {
                        Byte[] bytes = new Byte[1024];
                        int bytesReceived = _handler.Receive(bytes);

                        data.Append(Encoding.ASCII.GetString(bytes, 0, bytesReceived));

                        if (data.ToString().IndexOf("END###") > -1) {
                            OnFileReceived(new ReceivedFileEventArgs(data.ToString()));

                            _handler.Send(Encoding.ASCII.GetBytes("OK"));

                            data = new StringBuilder();
                        }

                        if (data.ToString().IndexOf("BYE###") > -1) {
                            receivingFile = false;
                        }
                    }

                    _handler.Shutdown(SocketShutdown.Both);
                    _handler.Close();
                }
            } catch (Exception e) {
                if ((_handler != null) && _handler.Connected) {
                    _handler.Shutdown(SocketShutdown.Both);
                    _handler.Close();
                }

                OnErrorEncountered(new ExceptionEventArgs(e));
            }

            OnClosedPort(new EventArgs());
        }
        
        protected virtual void OnBeginningFileReceipt(EventArgs e) {
            if (BeginningFileReceipt != null) {
                BeginningFileReceipt(this, e);
            }
        }

        protected virtual void OnClosedPort(EventArgs e) {
            if (ClosedPort != null) {
                ClosedPort(this, e);
            }
        }

        protected virtual void OnErrorEncountered(ExceptionEventArgs e) {
            if (ErrorEncountered != null) {
                ErrorEncountered(this, e);
            }
        }

        protected virtual void OnFileReceived(ReceivedFileEventArgs e) {
            if (FileReceived != null) {
                FileReceived(this, e);
            }
        }

        protected virtual void OnOpeningPort(EventArgs e) {
            if (OpeningPort != null) {
                OpeningPort(this, e);
            }
        }

        protected virtual void OnWaitingConnection(EventArgs e) {
            if (WaitingConnection != null) {
                WaitingConnection(this, e);
            }
        }
        #endregion
    }
}
