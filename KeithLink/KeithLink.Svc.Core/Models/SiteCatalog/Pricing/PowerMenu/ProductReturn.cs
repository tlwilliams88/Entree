using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    public class ProductReturn {
        public List<Product> Products;

        public ProductReturn() {
            Products = new List<Product>();
        }
    }
}
