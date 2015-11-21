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

        [DataMember( Name = "totalweeks" )]
        public int TotalWeeks { get; set; }

        [DataMember(Name = "caseaverage")]
        public int CaseAverage { get; set; }

        [DataMember( Name = "packageaverage" )]
        public int PackageAverage { get; set; }

        #endregion
    }
}
