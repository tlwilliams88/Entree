using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Models.Reports
{
    [DataContract(Name = "reportitemusage")]
    public class ItemUsageReportItemModel
    {
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "images")]
        public List<ProductImage> Images { get; set; }

        [DataMember(Name = "totalquantityordered")]
        public int TotalQuantityOrdered { get; set; }

        [DataMember(Name = "totalquantityshipped")]
        public int TotalQuantityShipped { get; set; }
    }
}
