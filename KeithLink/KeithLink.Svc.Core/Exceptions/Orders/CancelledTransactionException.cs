using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class CancelledTransactionException: Exception
    {
        public CancelledTransactionException() : base("Transaction was cancelled on the host.") { }
    }
}
