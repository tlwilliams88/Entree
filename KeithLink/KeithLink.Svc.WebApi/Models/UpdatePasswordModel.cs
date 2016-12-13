using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models {
    /// <summary>
    /// 
    /// </summary>
    public class UpdatePasswordModel {
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OriginalPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NewPassword { get; set; }
    }
}