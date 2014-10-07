using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders {
    [DataContract(Name="ShipDateReturn")]
    public class ShipDateReturn {
        #region ctor
        public ShipDateReturn() {
            ShipDays = new List<string>();
        }
        #endregion

        #region properties
        public string CutOffTime { get; set; }

        public List<string> ShipDays { get; set; }
        #endregion
    }
}
