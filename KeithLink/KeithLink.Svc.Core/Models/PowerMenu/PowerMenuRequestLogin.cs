using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("Admin")]
    public class PowerMenuSystemRequestAdminModel {

        public PowerMenuSystemRequestAdminModel() {
            AdminUsername = string.Empty;
            AdminPassword = string.Empty;
        }

        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
    }
}
