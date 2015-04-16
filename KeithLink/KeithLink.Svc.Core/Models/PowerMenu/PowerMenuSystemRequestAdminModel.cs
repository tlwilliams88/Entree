using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("Admin")]
    [DataContract(Name = "Admin", Namespace = "")]
    public class PowerMenuSystemRequestAdminModel {

        public PowerMenuSystemRequestAdminModel() {
            AdminUsername = string.Empty;
            AdminPassword = string.Empty;
        }

        [DataMember(Name = "AdminUsername")]
        public string AdminUsername { get; set; }

        [DataMember(Name = "AdminPassword")]
        public string AdminPassword { get; set; }
    }
}
