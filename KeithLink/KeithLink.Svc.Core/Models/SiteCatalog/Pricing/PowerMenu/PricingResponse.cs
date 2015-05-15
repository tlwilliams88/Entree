using System;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    [XmlType("GetProductsWithPriceResponse")]
    public class PricingResponse {
        #region ctor
        public PricingResponse() {
            //Results = new ProductReturn();
        }
        #endregion

        #region properties
        [XmlElement("GetProductsWithPriceResult")]
        public String Results { get; set; }
        //public ProductReturn Results { get; set; }
        #endregion
    }
}
