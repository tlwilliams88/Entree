using System;
using KeithLink.Svc.Core.Models.Profile;
using System.Collections.Generic;

namespace KeithLink.Svc.WebApi.Models
{
    /// <summary>
    /// AccountModel in WebApi
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// AccountId
        /// </summary>
        public Nullable<Guid> AccountId { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Customers
        /// </summary>
        public List<Customer> Customers { get; set; }
        /// <summary>
        /// Users
        /// </summary>
        public List<UserProfile> Users { get; set; }
    }
}