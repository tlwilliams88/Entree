using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [Serializable]
    public class InvoicePermissionsModel
    {
        [DataMember(Name = "canview")]
        public bool CanView { get; set; }
    }
}
