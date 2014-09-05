using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class EarlySocketException : Exception
    {
        #region ctor
        public EarlySocketException() { }

        public EarlySocketException(string Message) : base(Message) { }

        public EarlySocketException(string Message, Exception ex) : base(Message, ex) { }
        #endregion
    }
}
