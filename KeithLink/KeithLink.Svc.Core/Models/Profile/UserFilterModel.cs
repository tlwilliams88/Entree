using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class UserFilterModel
    {
        public Nullable<Guid> AccountId { get; set; }
        public Nullable<Guid> CustomerId { get; set; }
        public string Email { get; set; }
    }
}