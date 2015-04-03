using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Enumerations.SingleSignOn {
    [DataContract(Name="AccessRequestType")]
    public enum AccessRequestType {
        [EnumMember]
        Undefined = 0,

        [Description("KBIT Customer Access")]
        [EnumMember]
        KbitCustomer = 1,

        [Description("PowerMenu User")]
        [EnumMember]
        PowerMenu = 2
    }
}
