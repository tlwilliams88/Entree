using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Models.Customers
{
    [DataContract(Name = "growth_and_recovery")]
    public class GrowthAndRecoveriesModel
    {
        public enum GrowthAndRecoveryType {
            Growth = 1,
            Recovery = 2
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "productgroup")]
        public int GrowthAndRecoveryProductGroup { get; set; }

        [DataMember(Name = "code")]
        public string GroupingCode { get; set; }

        [DataMember(Name = "description")]
        public string GroupingDescription { get; set; }

        [DataMember(Name = "type")]
        public GrowthAndRecoveryType GrowthAndReccoveryTypeKey { get; set; }

        [DataMember(Name = "image")]
        public CategoryImageReturn Image { get; set; }
    }
}
