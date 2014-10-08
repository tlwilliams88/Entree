using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders {
    [DataContract(Name = "shipdate")]
    public class ShipDate {
        #region properties
        [DataMember(Name="cutoffdatetime")]
        public string CutOffDateTime { get; set; }

        [DataMember(Name = "shipdate")]
        public string Date { get; set; }

        [DataMember(Name = "dayofweek")]
        public string DayOfWeek { get; set; }
        #endregion
    }
}
