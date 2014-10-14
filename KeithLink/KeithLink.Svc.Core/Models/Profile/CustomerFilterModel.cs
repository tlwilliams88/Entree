using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class CustomerFilterModel
    {
        public string AccountId { get; set; }
        public string UserId { get; set; }
        public string Wildcard { get; set; }
    }
}