using System;
using KeithLink.Svc.Core.Models.Profile;
using System.Collections.Generic;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Nullable<Guid> AccountId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Customer> Customers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UserProfile> Users { get; set; }
    }
}