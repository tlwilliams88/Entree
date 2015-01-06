using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="ordereta")]
    public class OrderEta
    {
        [DataMember(Name="orderId")]
        public string OrderId { get; set; }

        [DataMember(Name = "scheduledTime")]
        public DateTime scheduledTime { get; set; }

        [DataMember(Name="estimatedTime")]
        public DateTime estimatedTime { get; set; }

        [DataMember(Name="routeId")]
        public string RouteId { get; set; }

        [DataMember(Name = "stopNumber")]
        public string StopNumber { get; set; }

        [DataMember(Name = "outOfSequence")]
        public bool OutOfSequence { get; set; }
    }
}
