using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Profile {
    public class ApplicationUser {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string HashedPassword { get; set; }

        public string Company { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmation { get; set; }

        public bool Enabled { get; set; }

        public bool Administrator { get; set; }

        public bool AcceptedAgreement { get; set; }
    }
}
