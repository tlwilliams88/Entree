using KeithLink.Svc.Core.Enumerations.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KeithLink.Svc.Core.Models.Messaging
{
    public class PushDeviceRegistrationModel
    {
        [DataMember(Name="providertoken")]
        public string ProviderToken { get; set; }
        [DataMember(Name="deviceid")]
        public string DeviceId { get; set; }
        [DataMember(Name="deviceos")]
        public DeviceOS DeviceOS { get; set; }
    }
}
