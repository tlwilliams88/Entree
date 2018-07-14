using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Entree.Core.SiteCatalog.Models.Pricing.PowerMenu {
    [XmlType("GetProductsWithPriceResult")]
    public class ProductReturn {
        public List<Product> Products;

        public ProductReturn() {
            Products = new List<Product>();
        }
    }
}
