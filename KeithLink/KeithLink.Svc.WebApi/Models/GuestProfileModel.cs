using System;
using System.Web;

namespace KeithLink.Svc.WebApi.Models {
    /// <summary>
    /// 
    /// </summary>
    public class GuestProfileModel {
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BranchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ExistingCustomer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool MarketingFlag { get; set; }
    }
}