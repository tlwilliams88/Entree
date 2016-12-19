using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Reports
{
    [DataContract]
    public class InventoryValuationModel
    {
        [DataMember(Name = "itemnumber")]
        public string ItemId { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "brand_extended_description")]
        public string Brand { get; set; }
        [DataMember(Name = "class")]
        public string Category { get; set; }
        [DataMember(Name = "category")]
        public string ContractCategory { get; set; }
        [DataMember(Name = "supplier")]
        public string Supplier { get; set; }
        [DataMember(Name = "quantity")]
        public decimal Quantity { get; set; }
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
        [DataMember(Name = "extprice")]
        public decimal ExtPrice { get; set; }
        [DataMember(Name = "each")]
        public bool Each { get; set; }
        [DataMember(Name = "packsize")]
        public string PackSize { get; set; }
        public string Pack { get; set; }
        public string Size { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        // Grouping is a groupby used internally for the case where they want to
        // group by contract category then label (if there is no contract category)
        public string Grouping { get; set; }
    }
}