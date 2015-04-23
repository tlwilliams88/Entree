using System;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    public class Product {
        public string ProductNumber;
        public bool IsAuthorized;
        public bool IsActive;
        public string PurchaseByUnit;
        public decimal AvailableQty;
        public decimal Price;
    }
}
