﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core
{
    [DataContract(Name = "product")]
    public class Product
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

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
        public string CategoryId;

        [DataMember(Name = "kosher")]
        public string Kosher { get; set; }

        [DataMember(Name = "price")]
        public string Price { get; set; }
    }

    [DataContract(Name = "ProductsReturn")]
    public class ProductsReturn
    {
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
    }
}
