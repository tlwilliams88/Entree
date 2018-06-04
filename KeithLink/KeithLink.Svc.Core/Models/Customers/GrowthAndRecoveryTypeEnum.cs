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
        [EnumMember(Value = "salesiq-growth")]
        Growth = 1,
        [EnumMember(Value = "salesiq-recovery")]
        Recovery = 2
    }
}
