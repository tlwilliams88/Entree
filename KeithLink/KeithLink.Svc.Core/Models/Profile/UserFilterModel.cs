using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class UserFilterModel
    {
        public string AccountId { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
    }
}