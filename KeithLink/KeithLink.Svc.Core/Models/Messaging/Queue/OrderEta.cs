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
        [DataMember(Name="orderid")]
        public string OrderId { get; set; }

        [DataMember(Name = "scheduledtime")]
        public string ScheduledTime { get; set; }

        [DataMember(Name="estimatedtime")]
        public string EstimatedTime { get; set; }

        [DataMember(Name = "actualtime")]
        public string ActualTime { get; set; }

        [DataMember(Name="routeid")]
        public string RouteId { get; set; }

        [DataMember(Name = "stopnumber")]
        public string StopNumber { get; set; }

        [DataMember(Name = "outofsequence")]
        public bool? OutOfSequence { get; set; }
        
        [DataMember(Name = "branch")]
        public string BranchId { get; set; }
    }
}
