using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging
{
    [DataContract(Name = "UserMessagingPreference")]
    public class UserMessagingPreferenceModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "userid")]
        public Guid UserId { get; set; }
        [DataMember(Name = "notificationtype")]
        public NotificationType NotificationType { get; set; }
        [DataMember(Name = "channel")]
        public Channel Channel { get; set; }

        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }
    }
}