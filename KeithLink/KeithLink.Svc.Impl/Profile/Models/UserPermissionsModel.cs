using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models
{
    [Serializable]
    public class UserPermissionsModel
    {
        public UserPermissionsModel()
        {
            Invoices = new Profile.InvoicePermissionsModel();
        }
        [DataMember(Name = "invoices")]
        public InvoicePermissionsModel Invoices { get; set; }
    }
}
