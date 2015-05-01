using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile {
    public class DsrAliasModel {
        public int DsrAliasId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string BranchId { get; set; }
        public string DsrNumber { get; set; }
    }
}
