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
        KbitCustomer = 1
    }

    public enum RequestedApplication{
        NotSet = 0,
        KbitCustomer = 1
    }
}
