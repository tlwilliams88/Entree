using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Messaging.Queue {
    [DataContract(Name = "paymentconfirmationnotification")]
    public class PaymentConfirmationNotification : BaseNotification {
        #region ctor
        public PaymentConfirmationNotification() {
            this.NotificationType = Enumerations.Messaging.NotificationType.PaymentConfirmation;
        }
        #endregion

        #region properties
        [DataMember(Name = "payments")]
        public List<PaymentTransactionModel> Payments { get; set; }
        #endregion
    }
}
