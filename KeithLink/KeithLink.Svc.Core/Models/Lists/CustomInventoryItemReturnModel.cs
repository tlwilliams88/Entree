using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.ComponentModel.DataAnnotations.Schema;


namespace KeithLink.Svc.Core.Models.Lists
{
    [Serializable]
    [DataContract(Name = "custominventoryitem")]
    public class CustomInventoryItemReturnModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }
        [IgnoreDataMember]
        public string CustomerNumber { get; set; }
        [IgnoreDataMember]
        public string BranchId { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "brand")]
        public string Brand { get; set; }
        [DataMember(Name = "supplier")]
        public string Supplier { get; set; }
        [DataMember(Name = "pack")]
        public string Pack { get; set; }
        [DataMember(Name = "size")]
        public string Size { get; set; }
        [DataMember(Name = "vendor")]
        public string Vendor { get; set; }
        [DataMember(Name = "each")]
        public bool Each { get; set; }
        [DataMember(Name = "caseprice")]
        public string CasePrice { get; set; }
        [DataMember(Name = "packageprice")]
        public string PackagePrice { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
    }
}
