using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models {
    /// <summary>
    /// UpdatePasswordModel
    /// </summary>
    public class UpdatePasswordModel {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// OriginalPassword
        /// </summary>
        public string OriginalPassword { get; set; }
        /// <summary>
        /// NewPassword
        /// </summary>
        public string NewPassword { get; set; }
    }
}