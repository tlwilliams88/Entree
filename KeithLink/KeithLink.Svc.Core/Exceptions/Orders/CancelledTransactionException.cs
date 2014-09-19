using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class CancelledTransactionException: Exception {
        #region ctor
        public CancelledTransactionException(int confirmationNumber) : base("Transaction was cancelled on the host.") {
            this.ConfirmationNumber = confirmationNumber;
        }
        #endregion

        #region properties
        public int ConfirmationNumber;
        #endregion
    }
}
