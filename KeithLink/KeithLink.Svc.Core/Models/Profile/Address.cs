using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="address")]
	[Serializable]
    public class Address
    {
        [DataMember(Name="street")]
        public string StreetAddress { get; set; }

        [DataMember(Name="city")]
        public string City { get; set; }

        [DataMember(Name="regioncode")]
        public string RegionCode { get; set; }

        [DataMember(Name="postalcode")]
        public string PostalCode { get; set; }
    }
}
