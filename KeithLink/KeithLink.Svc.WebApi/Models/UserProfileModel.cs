using System;
using System.Text;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Profile;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserProfileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set;}
        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public List<string> Permissions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "permit")]
        public UserPermissionsModel Permit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BranchId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Customer> Customers { get; set; }
    }
}