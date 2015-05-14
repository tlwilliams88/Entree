using System;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    //[SoapType(Namespace="http://benekeith.com")]
    public class Product {
        //[SoapAttribute]
        [XmlAttribute]
        public string ProductNumber;
        //[SoapAttribute]
        [XmlAttribute]
        public bool IsAuthorized;
        //[SoapAttribute]
        [XmlAttribute]
        public bool IsActive;
        //[SoapAttribute]
        [XmlAttribute]
        public string PurchaseByUnit;
        //[SoapAttribute]
        [XmlAttribute]
        public decimal AvailableQty;
        //[SoapAttribute]
        [XmlAttribute]
        public decimal Price;
    }
}
