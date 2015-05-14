using System;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    //[XmlRoot]
    //[SoapType("GetProductsWithPriceResponse")]
    [XmlType("GetProductsWithPriceResponse")]
    public class PricingResponse {
        #region ctor
        public PricingResponse() {
            Results = new ProductReturn();
        }
        #endregion

        #region properties
        //[SoapElement("GetProductsWithPriceResult")]
        [XmlElement("GetProductsWithPriceResult")]
        public ProductReturn Results { get; set; }
        #endregion
    }
}
