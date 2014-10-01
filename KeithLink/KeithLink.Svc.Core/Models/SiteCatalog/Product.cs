using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "product")]
    [Serializable]
    public class Product: BaseProductInfo
    {
		private string pack;

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

		[DataMember(Name = "pack")]
		public string Pack
		{
			get { return pack.TrimStart(new char[] { '0' }); }
			set { pack = value; }
		}

		[DataMember(Name = "nutritional")]
        public Nutritional Nutritional { get; set; }
        
        [DataMember(Name = "temp_zone")]
        public string TempZone { get; set; }

        [DataMember(Name = "catchweight")]
        public string Catchweight { get; set; }

        [DataMember(Name = "productimages")]
        public List<ProductImage> ProductImages { get; set; }

		[DataMember(Name = "isproprietary")]
		public bool IsProprietary { get; set; }
        #endregion

		[OnSerializing]
		void OnSerializing(StreamingContext context)
		{
			//Do not output a "blank" upc
			if (this.UPC.Equals("00000000000000"))
				this.UPC = string.Empty;
		}
    }

	
}
