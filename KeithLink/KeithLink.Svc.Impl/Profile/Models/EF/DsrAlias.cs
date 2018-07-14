using KeithLink.Svc.Core.Models.EF;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entree.Core.Profile.Models.EF {
	public class DsrAlias : BaseEFModel {
        [Required]
        [Index]
        public Guid UserId { get; set; }

        [Required]
        [Column(TypeName="varchar")]
        [MaxLength(200)]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName="char")]
        [MaxLength(3)]
        public string BranchId { get; set; }

        [Required]
        [Column(TypeName="char")]
        [MaxLength(6)]
        public string DsrNumber { get; set; }
    }
}
