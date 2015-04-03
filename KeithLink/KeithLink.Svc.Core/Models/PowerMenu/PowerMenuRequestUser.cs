using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("User")]
    public class PowerMenuSystemRequestUserModel {

        public PowerMenuSystemRequestUserModel() {
            Username = string.Empty;
            Password = string.Empty;
            CustomerNumber = string.Empty;
            EmailAddress = string.Empty;
            ContactName = string.Empty;
            PhoneNumber = string.Empty;
            State = string.Empty;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string CustomerNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
    }
}