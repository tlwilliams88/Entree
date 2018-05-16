using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    public enum GrowthAndRecoveryType
    {
        [EnumMember(Value = "growth")]
        Growth = 1,
        [EnumMember(Value = "recovery")]
        Recovery = 2
    }
}
