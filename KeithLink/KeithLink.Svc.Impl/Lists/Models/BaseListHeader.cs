using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Lists.Models {
    public abstract class BaseListHeader : AuditableEntity {
        public string BranchId { get; set; }
        public string CustomerNumber { get; set; }
    }
}
