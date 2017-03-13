using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
	[DataContract]
	public class ResetPasswordModel
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }
        private string _password;
        /// <summary>
        /// Password
        /// </summary>
		[DataMember(Name = "password")]
        public string Password
        {
            get
            {
                return WebUtility.UrlDecode(_password);
            }
            set
            {
                _password = value;
            }
        }
    }
}
