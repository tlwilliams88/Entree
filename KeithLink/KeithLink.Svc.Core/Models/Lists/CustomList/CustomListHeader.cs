using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Core.Models.Lists.CustomList
{
    public class CustomListHeader
    {
        public long Id { get; set; }
        public Guid? UserId { get; set; }
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
