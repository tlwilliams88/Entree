using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class AccountFilterModel
    {
        public Nullable<Guid> UserId { get; set; }
        public string Wildcard { get; set; }
    }
}