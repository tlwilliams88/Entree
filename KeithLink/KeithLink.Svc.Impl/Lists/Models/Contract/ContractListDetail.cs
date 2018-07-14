using System;

namespace Entree.Core.Lists.Models.Contract {
    public class ContractListDetail : BaseListDetail {
        public string Category { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
