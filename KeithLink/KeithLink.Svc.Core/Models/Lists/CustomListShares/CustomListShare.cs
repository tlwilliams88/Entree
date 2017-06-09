using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists.CustomListShares
{ 
    public class CustomListShare
    {
        public long Id { get; set; }
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
        public long ParentCustomListHeaderId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
