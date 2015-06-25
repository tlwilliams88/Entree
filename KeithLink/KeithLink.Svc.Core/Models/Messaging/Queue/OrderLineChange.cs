using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="orderline")]
    public class OrderLineChange
    {
        [DataMember(Name="itemnumber")]
        public string ItemNumber { get; set; }
        [DataMember(Name = "itemdescription")]
        public string ItemDescription { get; set; }
        [DataMember(Name = "substituteditemnumber")]
        public string SubstitutedItemNumber { get; set; }
        [DataMember(Name = "quantityordered")]
        public int QuantityOrdered { get; set; }
        [DataMember(Name = "quantityshipped")]
        public int QuantityShipped { get; set; }
        [DataMember(Name = "originalstatus")]
        public string OriginalStatus { get; set; }
        [DataMember(Name = "newstatus")]
        public string NewStatus { get; set; }
        [DataMember(Name = "itemprice")]
        public decimal ItemPrice { get; set; }
    }
}
