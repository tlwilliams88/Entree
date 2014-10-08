using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders {
    [DataContract(Name="ShipDateReturn")]
    public class ShipDateReturn {
        #region ctor
        public ShipDateReturn() {
            ShipDates = new List<ShipDate>();
        }
        #endregion

        #region properties
        [DataMember(Name="shipdates")]
        public List<ShipDate> ShipDates { get; set; }
        #endregion
    }
}
