using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class SocketSendException : Exception
    {
        #region ctor
        public SocketSendException(string ExceptionMessage) : base(ExceptionMessage) { }

        public SocketSendException(string ExceptionMessage, Exception ex) : base(ExceptionMessage, ex) { }
        #endregion
    }
}
