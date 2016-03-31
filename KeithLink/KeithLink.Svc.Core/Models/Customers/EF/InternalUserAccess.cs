// KeithLink
using KeithLink.Svc.Core.Models.EF;

// Core
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers.EF {
    public class InternalUserAccess : BaseEFModel {

        [Required]
        [MaxLength( 3 )]
        [Column( TypeName = "char" )]
        [Index( "IdxInternalUser", 1 )]
        public string BranchId { get; set; }

        [MaxLength( 6 )]
        [Column( TypeName = "char" )]
        [Index( "IdxInternalUser", 2 )]
        public string CustomerNumber { get; set; }

        [Required]
        [Index( "IdxByUserId")]
        public Guid UserId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [MaxLength( 70 )]
        [Column( TypeName = "varchar" )]
        public string RoleId { get; set; }

        [Required]
        [MaxLength( 200 )]
        [Column( TypeName = "varchar" )]
        public string EmailAddress { get; set; }

    }
}
