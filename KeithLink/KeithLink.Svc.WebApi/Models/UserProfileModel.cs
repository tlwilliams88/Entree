using System;
using System.Text;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Profile;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// UserProfileModel
    /// </summary>
    public class UserProfileModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set;}
        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// CustomerName
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }
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
        /// Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        [JsonIgnore]
        public List<string> Permissions { get; set; }
        /// <summary>
        /// Permit
        /// </summary>
        [DataMember(Name = "permit")]
        public UserPermissionsModel Permit { get; set; }

        /// <summary>
        /// BranchId
        /// </summary>
        public string BranchId { get; set; }
        /// <summary>
        /// Customers
        /// </summary>
        public List<Customer> Customers { get; set; }
    }
}