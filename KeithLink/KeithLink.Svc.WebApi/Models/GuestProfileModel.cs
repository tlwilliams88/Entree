using System;
using System.Web;

namespace KeithLink.Svc.WebApi.Models {
    public class GuestProfileModel {
        public string Email { get; set; }
        public string Password { get; set; }
        public string BranchId { get; set; }
        public bool ExistingCustomer { get; set; }
        public bool MarketingFlag { get; set; }
    }
}