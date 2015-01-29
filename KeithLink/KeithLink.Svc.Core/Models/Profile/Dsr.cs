using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile {
    [DataContract(Name = "dsr")]
    public class Dsr {

        [DataMember(Name = "dsrnumber")]
        public string DsrNumber { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember( Name = "phonenumber" )]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "emailaddress")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "imageurl")]
        public string ImageUrl { get; set; }
        
    }
}
