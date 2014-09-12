using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Interface.Common;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;


namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class OrderSocketConnectionRepositoryImpl : ISocketConnectionRepository, IDisposable
    {
        #region attributes
        private TcpClient _clientConnection;
        private bool _connected;
        private bool _disposed;
        private NetworkStream _netStream;
        private BinaryReader _readingStream;
        private BinaryWriter _writingStream;
        #endregion

        #region ctor
        public OrderSocketConnectionRepositoryImpl()
        {
            _connected = false;
            _disposed = false;
        }
        #endregion

        #region methods

        public void Close()
        {
            if (_connected)
            {
                try
                {
                    _writingStream.Close();
                    _readingStream.Close();
                    _netStream.Close();

                    if (_clientConnection.Connected) { _clientConnection.Close(); }

                    _connected = false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not close socket connection", ex);
                }
            }
        }

        public void Connect()
        {
            if (!_connected)
            {
                try
                {
                    _clientConnection = new TcpClient(Configuration.MainframeIPAddress, Configuration.MainframeListeningPort);
                    _clientConnection.ReceiveTimeout = 60000;

                    _netStream = _clientConnection.GetStream();
                    _readingStream = new BinaryReader(_netStream);
                    _writingStream = new BinaryWriter(_netStream);
                }
                catch (Exception ex)
                {
                    throw new EarlySocketException("Could not open socket", ex);
                }

                _connected = true;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Close();

                if (_netStream != null) { _netStream.Dispose(); }
            }

            _disposed = true;
        }

        public string Receive()
        {
            //putting dummy records in the bytes field at first to keep it from being null when reading in because that caused an exception
            byte[] bytes = Encoding.ASCII.GetBytes("HI");

            try
            {
                _readingStream.Read(bytes, 0, 2);
            }
            catch (Exception ex)
            {
                throw new SocketResponseException("Error reading from host socket connection", ex);
            }

            return ASCIIEncoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public void Send(string dataRecord)
        {
            if (!_connected) { throw new ApplicationException("Cannot send because a connection has not been established"); }
            if (dataRecord.Length > Constants.MAINFRAME_ORDER_RECORD_LENGTH) {
                throw new ArgumentException(string.Format("Record length exceeds the max size of {0} characters.", Constants.MAINFRAME_ORDER_RECORD_LENGTH));
            }

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(dataRecord.PadRight(Constants.MAINFRAME_ORDER_RECORD_LENGTH));

            try { _writingStream.Write(bytes); }
            catch (Exception ex)
            {
                throw new SocketSendException("Error sending record to host socket connection", ex);
            }
        }

        public void StartTransaction(string confirmationNumber)
        {
            if (!_connected) { throw new ApplicationException("Cannot send because a connection has not been established"); }

            StringBuilder cmd = new StringBuilder();
            cmd.Append(Configuration.MainframeOrderTransactionId);
            cmd.Append("                 ");
            cmd.Append(confirmationNumber);
            cmd.Length = 50;

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(cmd.ToString());

            try { _writingStream.Write(bytes); }
            catch (Exception ex)
            {
                throw new EarlySocketException("Error creating transaction", ex);
            }
        }

        #endregion
    }
}
