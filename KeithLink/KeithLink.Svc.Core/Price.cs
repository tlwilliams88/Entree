using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core
{
    [DataContract(Name="Price")]
    class Price
    {
        [DataMember(Name="BranchId")]
        public string BranchId { get; set; }

        [DataMember(Name="CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name="ItemNumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name="CasePrice")]
        public double CasePrice { get; set; }

        [DataMember(Name="PackagePrice")]
        public double PackagePrice { get; set; }
    }

    [DataContract(Name="PriceReturn")]
    class PriceReturn
    {
        [DataMember(Name="Prices")]
        public List<Price> Prices { get; set; }
    }
}
