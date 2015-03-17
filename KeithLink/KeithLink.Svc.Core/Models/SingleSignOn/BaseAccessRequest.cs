using KeithLink.Svc.Core.Enumerations.SingleSignOn;
using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SingleSignOn {
    [DataContract]
    public class BaseAccessRequest {
        [DataMember(Name="username")]
        public string UserName { get; set; }

        [DataMember(Name = "requesttype")]
        public AccessRequestType RequestType { get; set; }
    }
}
