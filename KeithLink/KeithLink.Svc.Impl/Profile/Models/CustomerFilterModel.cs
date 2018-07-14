using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entree.Core.Profile.Models
{
    public class CustomerFilterModel
    {
        public string AccountId { get; set; }
        public string UserId { get; set; }
        public string Wildcard { get; set; }
    }
}