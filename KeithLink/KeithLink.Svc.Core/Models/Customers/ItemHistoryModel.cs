// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers {
    [DataContract]
    public class ItemHistoryModel {
        #region properties

        [DataMember(Name = "caseeightweekaverage")]
        public int CaseEightWeekAverage { get; set; }

        [DataMember( Name = "packageeightweekaverage" )]
        public int PackageEightWeekAverage { get; set; }

        #endregion
    }
}
