using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product: BaseProductInfo
    {
        #region ctor
        public Product()
        {
            ProductImages = new List<ProductImage>();
        }
        #endregion

        #region properties
        
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "ext_description")]
        public string ExtendedDescription { get; set; }
		       
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

        [DataMember(Name= "vendor_num")]
        public string VendorItemNumber {get;set;}

        [DataMember(Name= "class")]
        public string ItemClass { get; set; }
                
        [DataMember(Name = "cube")]
        public string CaseCube { get; set; }

		[DataMember(Name="pack")]
		public string Pack { get; set; }

		[DataMember(Name = "gs1")]
        public Gs1 Gs1 { get; set; }

        [DataMember(Name = "productimages")]
        public List<ProductImage> ProductImages { get; set; }
        #endregion
    }
}
