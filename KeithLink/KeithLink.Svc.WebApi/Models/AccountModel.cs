using System;
using KeithLink.Svc.Core.Models.Profile;
using System.Collections.Generic;

namespace KeithLink.Svc.WebApi.Models
{
    public class AccountModel
    {
        public Nullable<Guid> AccountId { get; set; }
        public string Name { get; set; }
        public List<Customer> Customers { get; set; }
        public List<UserProfile> Users { get; set; }
    }
}