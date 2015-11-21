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
		[DataMember(Name = "customername")]
		public string CustomerName { get; set; }
		[DataMember(Name = "branch")]
		public string BranchId { get; set; }
		[DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "notificationtype")]
        public NotificationType NotificationType { get; set; }
		[DataMember(Name = "notificationtypedescription")]
		public string NotificationTypeDescription { get; set; }
        [DataMember(Name = "messageread")]
        public DateTime? MessageRead { get; set; }
        [DataMember(Name = "messagecreated")]
        public DateTime? MessageCreated { get; set; }
        [DataMember(Name = "subject")]
        public string Subject { get; set; }
        [DataMember(Name = "body")]
        public string Body { get; set; }
        [DataMember(Name = "mandatory")]
        public bool Mandatory { get; set; }
    }
}