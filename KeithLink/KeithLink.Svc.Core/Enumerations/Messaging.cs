using System;
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
		OrderConfirmation,
        [Description("My Order is Shipped")]
		[EnumMember]
		OrderUpdate,
        [Description("My Invoices Need Attention")]
		[EnumMember]
		InvoiceAttention,
        [Description("Ben E. Keith Has News For Me")]
		[EnumMember]
		HasNews
	}

	[DataContract(Name = "Channel")]
    public enum Channel : int
    {
        [Description("Email")]
		[EnumMember(Value="Email")]
        Email,
        [Description("Mobile Push")]
		[EnumMember(Value = "MobilePush")]
        MobilePush,
        [Description("Web")]
		[EnumMember(Value = "Web")]
        Web
    }
}
