using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments {
    public class State {
        [Key, Column("State", TypeName="char"), MaxLength(2)]
        public string StateId { get; set; }

        [MaxLength(30), Required]
        public string Name { get; set; }
    }
}
