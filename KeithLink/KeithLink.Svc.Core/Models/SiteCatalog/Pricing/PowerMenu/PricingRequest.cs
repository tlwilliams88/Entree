using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog.Pricing.PowerMenu {
    public class PricingRequest {
        #region ctor
        public PricingRequest() {
            products = new List<ProductLine>();
        }
        #endregion

        #region properties
        public string customerNumber { get; set; }

        public List<ProductLine> products { get; set; }

        public string effDate { get; set; }

        public bool getAllFields { get; set; }
        #endregion
    }
}
