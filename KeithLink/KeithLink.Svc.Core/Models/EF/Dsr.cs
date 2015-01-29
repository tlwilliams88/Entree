using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF {
    public class Dsr : BaseEFModel {
        public string DsrNumber { get; set; }
        public string EmailAddress { get; set; }
        public string BranchId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; } 
    }
}
