﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="orderconfirmationnotification")]
    public class OrderConfirmationNotification : BaseNotification
    {
        public OrderConfirmationNotification()
        {
            this.NotificationType = NotificationType.OrderConfirmation;
        }

        [DataMember(Name = "ordernumber")]
        public string OrderNumber { get; set; }

        [DataMember(Name = "invoicenumber")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "useremailaddress")]
        public string UserEmailAddress { get; set; }

        [DataMember(Name = "orderchange")]
        public OrderChange OrderChange { get; set; }
    }
}
