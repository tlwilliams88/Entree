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
        private string _password;
        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get
            {
                return System.Web.HttpUtility.UrlDecode(_password);
            }
            set
            {
                _password = value;
            }
        }
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