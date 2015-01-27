using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    public class CustomerAddUserModel
    {
        public Guid customerId { get; set; }
        public Guid userId { get; set; }
        public string role { get; set; }
    }
}