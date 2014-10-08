using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Orders {
    [DataContract(Name = "shipdate")]
    public class ShipDate {
        #region properties
        [DataMember(Name="cutoffdatetime")]
        public DateTime CutOffDateTime { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "dayofweek")]
        public string DayOfWeek { get; set; }
        #endregion
    }
}
