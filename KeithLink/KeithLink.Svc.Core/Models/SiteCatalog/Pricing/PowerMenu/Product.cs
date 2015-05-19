using System;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    public class Product {
        [XmlAttribute("Number")]
        public string ProductNumber;
        [XmlAttribute]
        public bool IsAuthorized;
        [XmlAttribute("Active")]
        public bool IsActive;
        [XmlAttribute("PBUnit")]
        public string PurchaseByUnit;
        [XmlAttribute("IsCatchwgt")]
        public bool IsCatchWeight;
        [XmlIgnore]
        public decimal AvailableQty;
        [XmlAttribute]
        public decimal Price;
    }
}
