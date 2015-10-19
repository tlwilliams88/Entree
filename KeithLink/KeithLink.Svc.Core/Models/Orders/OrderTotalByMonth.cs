using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders {
    [DataContract(Name = "monthlysummary")]
    public class OrderTotalByMonth {

        public OrderTotalByMonth() {
            Totals = new List<double>();
        }

        [DataMember(Name = "totals")]
        public List<double> Totals { get; set; }

    }
}
