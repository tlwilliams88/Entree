﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Messaging
{
	[DataContract(Name = "NotificationType")]
	public enum NotificationType
	{
        [Description("My Order is Confirmed")]
		[EnumMember]
		OrderConfirmation = 1,
        [Description("My Order is Shipped")]
		[EnumMember]
		OrderUpdate = 2,
        [Description("My Invoices Need Attention")]
		[EnumMember]
		InvoiceAttention = 4,
        [Description("Ben E. Keith Has News For Me")]
		[EnumMember]
		HasNews = 8
	}

	[DataContract(Name = "Channel")]
    public enum Channel : int
    {
        [Description("Email")]
		[EnumMember(Value="Email")]
        Email = 1,
        [Description("Mobile Push")]
		[EnumMember(Value = "MobilePush")]
        MobilePush = 2,
        [Description("Web")]
		[EnumMember(Value = "Web")]
        Web = 4
    }
}
