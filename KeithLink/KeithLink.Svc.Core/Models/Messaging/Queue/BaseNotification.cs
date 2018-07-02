using KeithLink.Svc.Core.Enumerations.Messaging;

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract]
    public class BaseNotification
    {
        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        [DataMember(Name="customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "notificationtype")]
        public NotificationType NotificationType { get; set; }

        [DataMember(Name = "audience")]
        public Audience Audience { get; set; }

        [DataMember(Name = "dsrdsmonly")]
		public bool DSRDSMOnly { get; set; }
    }
}
