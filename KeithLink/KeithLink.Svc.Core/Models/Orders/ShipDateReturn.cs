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
        [DataMember(Name="cutofftime")]
        public string CutOffTime { get; set; }

        [DataMember(Name="shipdays")]
        public List<string> ShipDays { get; set; }
        #endregion
    }
}
