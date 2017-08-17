using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    public class ContractChange
    {
        public string CustomerNumber { get; set; }

        public string BranchId { get; set; }

        public string ItemNumber { get; set; }

        public bool Each { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime ModifiedUtc { get; set; }

        public string CatalogId { get; set; }

        public string Status { get; set; }
    }
}
