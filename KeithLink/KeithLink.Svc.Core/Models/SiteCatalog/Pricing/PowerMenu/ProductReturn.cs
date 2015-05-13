using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    //[SoapInclude(typeof(List<Product>))]
    //[SoapType(Namespace = "http://benekeith.com", TypeName="GetProductsWithPriceResult")]
    [XmlType("GetProductsWithPriceResult")]
    public class ProductReturn {
        public List<Product> Products;

        public ProductReturn() {
            Products = new List<Product>();
        }
    }
}
