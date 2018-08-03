using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class HostTransactionFailureException : Exception
    {
        #region ctor
        public HostTransactionFailureException(string ExceptionMessage) : base(ExceptionMessage) { }

        public HostTransactionFailureException(string ExceptionMessage, Exception ex) : base(ExceptionMessage, ex) { }
        #endregion
    }
}
