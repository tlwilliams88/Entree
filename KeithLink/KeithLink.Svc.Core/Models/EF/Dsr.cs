using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF {
    public class Dsr : BaseEFModel {
        [Column(TypeName="char"), MaxLength(3)]
        public string DsrNumber { get; set; }

        [Column(TypeName="varchar"), MaxLength(200)]
        public string EmailAddress { get; set; }

        [Column(TypeName="char"), MaxLength(3)]
        public string BranchId { get; set; }

        [Column(TypeName="nvarchar"), MaxLength(50)]
        public string Name { get; set; }

        [Column(TypeName="nvarchar"), MaxLength(50)]
        public string Phone { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(200)]
        public string ImageUrl { get; set; } 
    }
}
