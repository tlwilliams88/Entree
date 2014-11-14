using KeithLink.Svc.Core.Enumerations.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KeithLink.Svc.Core.Models.Messaging
{
    public class ProfileMessagingPreferenceModel
    {
        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }
        [DataMember(Name = "preferences")]
        public List<ProfileMessagingPreferenceDetailModel> Preferences { get; set; }
    }
}
