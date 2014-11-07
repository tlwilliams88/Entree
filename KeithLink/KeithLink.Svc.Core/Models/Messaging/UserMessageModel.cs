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
    [DataContract(Name = "UserMessage")]
    public class UserMessageModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "userid")]
        public Guid UserId { get; set; }
        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }
        [DataMember(Name = "ordernumber")]
        public string OrderNumber { get; set; }
        [DataMember(Name = "notificationtype")]
        public NotificationType NotificationType { get; set; }
        [DataMember(Name = "messagereadutc")]
        public DateTime? MessageReadUtc { get; set; }
        [DataMember(Name = "messagecreatedutc")]
        public DateTime? MessageCreatedUtc { get; set; }
        [DataMember(Name = "subject")]
        public string Subject { get; set; }
        [DataMember(Name = "body")]
        public string Body { get; set; }
        [DataMember(Name = "mandatory")]
        public bool Mandatory { get; set; }
    }
}