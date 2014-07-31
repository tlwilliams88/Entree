using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Catalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "ext_description")]
        public string ExtendedDescription { get; set; }

        [DataMember(Name = "brand")]
        public string Brand { get; set; }

        [DataMember(Name = "size")]
        public string Size { get; set; }

        [DataMember(Name = "upc")]
        public string UPC { get; set; }

        [DataMember(Name = "manufacturer_number")]
        public string ManufacturerNumber { get; set; }

        [DataMember(Name = "manufacturer_name")]
        public string ManufacturerName { get; set; }

        [DataMember(Name = "cases")]
        public string Cases { get; set; }

        [DataMember(Name = "categoryId")]
        public string CategoryId { get; set; }

        [DataMember(Name = "categoryname")]
        public string CategoryName { get; set; }

        [DataMember(Name = "kosher")]
        public string Kosher { get; set; }

        [DataMember(Name = "caseprice")]
        public string CasePrice { get; set; }

        [DataMember(Name = "packageprice")]
        public string PackagePrice { get; set; }

        [DataMember(Name = "replacementitem")]
        public string ReplacementItem { get; set; }

        [DataMember(Name = "replaceditem")]
        public string ReplacedItem {get;set;}

        [DataMember(Name = "cndoc")]
        public string CNDoc { get; set; }

        [DataMember(Name = "gs1")]
        public Gs1 Gs1 { get; set; }
    }
}
