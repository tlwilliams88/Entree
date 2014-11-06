using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="orderchange")]
    public class OrderChange
    {
        [DataMember(Name="ordername")]
        public string OrderName { get; set; }

        [DataMember(Name = "originalstatus")]
        public string OriginalStatus { get; set; }

        [DataMember(Name="currentstatus")]
        public string CurrentStatus { get; set; }

        [DataMember(Name="itemchanges")]
        public List<OrderLineChange> ItemChanges { get; set; }

        [DataMember(Name = "items")]
        public List<OrderLineChange> Items { get; set; }
    }
}
