using System;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    public class Product {
        [XmlAttribute]
        public string ProductNumber;
        [XmlAttribute]
        public bool IsAuthorized;
        [XmlAttribute]
        public bool IsActive;
        [XmlAttribute]
        public string PurchaseByUnit;
        [XmlAttribute]
        public decimal AvailableQty;
        [XmlAttribute]
        public decimal Price;
    }
}
