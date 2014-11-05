using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name="orderlinechange")]
    public class OrderLineChange
    {
        public string OriginalItemNumber { get; set; }
        public string SubstitutedItemNumber { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityShipped { get; set; }
        public string OriginalStatus { get; set; }
        public string NewStatus { get; set; }
    }
}
