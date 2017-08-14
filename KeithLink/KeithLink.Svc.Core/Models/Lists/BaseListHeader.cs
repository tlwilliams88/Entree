using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists {
    public abstract class BaseListHeader : AuditableEntity {
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
    }
}
