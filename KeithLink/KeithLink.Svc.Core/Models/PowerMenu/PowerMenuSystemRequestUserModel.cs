using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("User")]
    [DataContract(Name = "User", Namespace = "")]
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

        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string CustomerNumber { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string State { get; set; }
    }
}