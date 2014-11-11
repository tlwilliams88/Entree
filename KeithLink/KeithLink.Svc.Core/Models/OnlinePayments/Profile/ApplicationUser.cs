using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Profile {
    public class ApplicationUser {
        [Key, MaxLength(30)]
        public string UserName { get; set; }

        [MaxLength(30), Required]
        public string FirstName { get; set; }

        [MaxLength(30), Required]
        public string LastName { get; set; }

        [MaxLength(50), Required]
        public string EmailAddress { get; set; }

        [MaxLength(32), Required]
        public string HashedPassword { get; set; }

        [MaxLength(50)]
        public string Company { get; set; }

        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        public bool EmailConfirmation { get; set; }

        public bool Enabled { get; set; }

        public bool Administrator { get; set; }

        public bool AcceptedAgreement { get; set; }
    }
}
