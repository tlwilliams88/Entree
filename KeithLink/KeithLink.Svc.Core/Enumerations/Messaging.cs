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
		//[Description("Undefined")] - no need for description; no preference for this type
		[EnumMember]
		Undefined = 0,
        [Description("My Order is Confirmed")]
		[EnumMember]
		OrderConfirmation = 1,
        [Description("My Order is Shipped")]
		[EnumMember]
		OrderShipped = 2,
        [Description("My Invoices Need Attention")]
		[EnumMember]
		InvoiceAttention = 4,
        [Description("Ben E. Keith Has News For Me")]
		[EnumMember]
		HasNews = 8,
		//[Description("Mail")] - no need for description; no preference for this type
		[EnumMember]
		Mail = 16,
        //[Description("ETA")] - no need for description; no preference for this type
        [EnumMember]
        Eta = 32
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

    [DataContract(Name = "DeviceOS")]
    public enum DeviceOS : int
    {
        [Description("iOS")]
        [EnumMember(Value="iOS")]
        iOS = 1,
        [Description("Android")]
        [EnumMember(Value="Android")]
        Android = 2
    }
}
