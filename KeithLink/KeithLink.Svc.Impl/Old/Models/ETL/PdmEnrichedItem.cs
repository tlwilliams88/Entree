using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.ETL {
    public class PdmEnrichedItem {
        public string ItemNumber { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public string Desription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedUTC { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ModifiedUTC { get; set; }
    }
}
