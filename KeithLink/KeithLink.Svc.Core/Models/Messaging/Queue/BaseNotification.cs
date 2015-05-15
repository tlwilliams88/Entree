using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
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

		[DataMember(Name = "dsrdsmonly")]
		public bool DSRDSMOnly { get; set; }
    }
}
