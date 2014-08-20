using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
    [DataContract(Name = "PriceReturn")]
    public class PriceReturn
    {

        #region ctor
        public PriceReturn()
        {
            Prices = new List<Price>();
        }
        #endregion

        #region properties
        [DataMember(Name = "Prices")]
        public List<Price> Prices { get; set; }
        #endregion
    }
}
