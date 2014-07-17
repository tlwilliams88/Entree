using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.ETL
{
    public class CSProduct
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pack { get; set; }
        public string Size { get; set; }
        public string MfrNumber { get; set; }
        public string MfrName { get; set; }
        public string UPC { get; set; }
        public string Brand { get; set; }
    }
}
