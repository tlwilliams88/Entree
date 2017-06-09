using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.Contract
{
    public class ContractListHeader
    {
        public long Id { get; set; }
        public string ContractId { get; set; }
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
