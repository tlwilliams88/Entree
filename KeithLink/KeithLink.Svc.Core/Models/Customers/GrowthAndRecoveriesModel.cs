using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Customers
{
    public class GrowthAndRecoveriesModel
    {
        public enum GrowthAndRecoveryType {
            Growth = 1,
            Recovery = 2
        }

        public int Id { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public int GrowthAndRecoveryProductGroup { get; set; }
        public string GroupingCode { get; set; }
        public string GroupingDescription { get; set; }
        public GrowthAndRecoveryType GrowthAndReccoveryTypeKey { get; set; }
    }
}
