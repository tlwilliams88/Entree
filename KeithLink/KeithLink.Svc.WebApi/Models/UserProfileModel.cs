using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    public class UserProfileModel
    {
        #region attributes
        private StringBuilder _email;
        private StringBuilder _firstName;
        private StringBuilder _lastName;
        private StringBuilder _customerName;
        private StringBuilder _phone;
        private StringBuilder _password;
        private StringBuilder _roleName;
        #endregion

        #region ctor
        public UserProfileModel()
        {
            _email = new StringBuilder();
            _firstName = new StringBuilder();
            _lastName = new StringBuilder();
            _customerName = new StringBuilder();
            _phone = new StringBuilder();
            _password = new StringBuilder();
            _roleName = new StringBuilder();
        }
        #endregion

        #region properties
        public string Email
        {
            get { return _email.ToString(); }
            set { _email = new StringBuilder(value); }
        }

        public string FirstName
        {
            get { return _firstName.ToString(); }
            set { _firstName = new StringBuilder(value); }
        }

        public string LastName
        {
            get { return _lastName.ToString(); }
            set { _lastName = new StringBuilder(value); }
        }

        public string CustomerName
        {
            get { return _customerName.ToString(); }
            set { _customerName = new StringBuilder(value); }
        }

        public string Phone
        {
            get { return _phone.ToString(); }
            set { _phone = new StringBuilder(value); }
        }

        public string Password
        {
            get { return _password.ToString(); }
            set { _password = new StringBuilder(value); }
        }

        public string RoleName
        {
            get { return _roleName.ToString(); }
            set { _roleName = new StringBuilder(value); }
        }
        #endregion
    }
}