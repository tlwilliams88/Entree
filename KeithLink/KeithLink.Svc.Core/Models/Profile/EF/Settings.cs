// KeithLink
using KeithLink.Svc.Core.Models.EF;

// Core
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.Profile.EF {
    public class Settings : BaseEFModel {

        [Required]
        [Index]
        public Guid UserId { get; set; }

        [Required]
        [Column( TypeName = "varchar" )]
        [MaxLength( 100 )]
        public string Key { get; set; }

        [Required]
        [Column( TypeName = "varchar" )]
        [MaxLength( 250 )]
        public string Value { get; set; }

    }
}
