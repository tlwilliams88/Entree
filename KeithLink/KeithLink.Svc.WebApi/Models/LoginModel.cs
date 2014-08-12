using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
	public class LoginModel
    {
        #region attributes
        private StringBuilder _email;
        private StringBuilder _password;
        #endregion

        #region ctor
        public LoginModel()
        {
            _email = new StringBuilder();
            _password = new StringBuilder();
        }
        #endregion

        #region properties
        public string Email {
            get { return _email.ToString(); }
            set { _email = new StringBuilder(value); }
        }

		public string Password {
            get { return _password.ToString(); }
            set { _password = new StringBuilder(value); }
        }
        #endregion
	}
}