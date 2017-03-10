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
        private string _originalPassword;
        /// <summary>
        /// OriginalPassword
        /// </summary>
        public string OriginalPassword
        {
            get
            {
                return System.Web.HttpUtility.UrlDecode(_originalPassword);
            }
            set
            {
                _originalPassword = value;
            }
        }
        private string _newPassword;
        /// <summary>
        /// NewPassword
        /// </summary>
        public string NewPassword
        {
            get
            {
                return System.Web.HttpUtility.UrlDecode(_newPassword);
            }
            set
            {
                _newPassword = value;
            }
        }
    }
}