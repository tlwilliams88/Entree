using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// LoginModel
    /// </summary>
	public class LoginModel
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email {get;set;}
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
    }
}