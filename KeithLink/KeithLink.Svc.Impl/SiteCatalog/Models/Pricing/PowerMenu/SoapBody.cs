using System;
using System.Xml.Serialization;

namespace Entree.Core.SiteCatalog.Models.Pricing.PowerMenu {
    [XmlRoot("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapBody {
        #region ctor
        public SoapBody() {
            Response = new PricingResponse();
        }
        #endregion

        #region properties
        [XmlElement(ElementName="GetProductsWithPriceResponse", Namespace="http://benekeith.com")]
        public PricingResponse Response { get; set; }
        #endregion
    }
}
