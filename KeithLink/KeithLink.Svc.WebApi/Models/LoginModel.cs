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
        /// <summary>
        /// Password
        /// </summary>
        public string Password {get;set;}
	}
}