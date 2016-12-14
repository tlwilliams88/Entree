using System;
using System.Web;

namespace KeithLink.Svc.WebApi.Models {
    /// <summary>
    /// GuestProfileModel
    /// </summary>
    public class GuestProfileModel {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// BranchId
        /// </summary>
        public string BranchId { get; set; }
        /// <summary>
        /// ExistingCustomer
        /// </summary>
        public bool ExistingCustomer { get; set; }
        /// <summary>
        /// MarketingFlag
        /// </summary>
        public bool MarketingFlag { get; set; }
    }
}