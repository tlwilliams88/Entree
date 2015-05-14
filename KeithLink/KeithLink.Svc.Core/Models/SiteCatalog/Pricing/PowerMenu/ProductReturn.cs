using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    [XmlType("GetProductsWithPriceResult")]
    public class ProductReturn {
        public List<Product> Products;

        public ProductReturn() {
            Products = new List<Product>();
        }
    }
}
