using System;

namespace KeithLink.Svc.Core.Models.Lists.Contract {
    public class ContractListDetail : BaseListDetail {
        public string Category { get; set; }
        public DateTime? FromDate { get; set; }
        public int LineNumber { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
