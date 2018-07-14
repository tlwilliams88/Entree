using System;
using System.Xml.Serialization;

namespace Entree.Core.SiteCatalog.Models.Pricing.PowerMenu {
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
