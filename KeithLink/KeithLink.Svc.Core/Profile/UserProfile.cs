using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    [DataContract(Name="UserProfile")]
    public class UserProfile
    {
        #region attributes
        private StringBuilder _userId;
        private StringBuilder _userName;
        private StringBuilder _firstName;
        private StringBuilder _lastName;
        private StringBuilder _emailAddress;
        private StringBuilder _phoneNumber;
        private CustomerContainer _customer;
        #endregion

        #region ctor
        public UserProfile()
        {
            _userId = new StringBuilder();
            _userName = new StringBuilder();
            _firstName = new StringBuilder();
            _lastName = new StringBuilder();
            _emailAddress = new StringBuilder();
            _phoneNumber = new StringBuilder();
            _customer = new CustomerContainer();
        }
        #endregion

        #region properties
        [DataMember(Name="UserId")]
        public string UserId
        {
            get { return _userId.ToString(); }
            set { _userId = new StringBuilder(value); }
        }

        [DataMember(Name = "UserName")]
        public string UserName
        {
            get { return _userName.ToString(); }
            set { _userName = new StringBuilder(value); }
        }

        [DataMember(Name="FirstName")]
        public string FirstName
        {
            get { return _firstName.ToString(); }
            set { _firstName = new StringBuilder(value); }
        }

        [DataMember(Name="LastName")]
        public string LastName
        {
            get { return _lastName.ToString(); }
            set { _lastName = new StringBuilder(value); }
        }

        [DataMember(Name="EmailAddress")]
        public string EmailAddress
        {
            get { return _emailAddress.ToString(); }
            set { _emailAddress = new StringBuilder(value); }
        }

        [DataMember(Name="PhoneNumber")]
        public string PhoneNumber
        {
            get { return _phoneNumber.ToString(); }
            set { _phoneNumber = new StringBuilder(value); }
        }

        [DataMember(Name="CustomerName")]
        public string CustomerName
        {
            get { return _customer.Name; }
            set { _customer.Name = value; }
        }
        #endregion
    }
}
