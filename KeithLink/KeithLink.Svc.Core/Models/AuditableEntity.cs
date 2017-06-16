using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models {
    public abstract class AuditableEntity : Entity {
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
