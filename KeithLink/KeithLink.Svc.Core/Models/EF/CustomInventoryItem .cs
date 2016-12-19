using KeithLink.Svc.Core.Models.Lists;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.EF {
    public class CustomInventoryItem : BaseEFModel {
        [Required]
        [MaxLength(25)]
        public string ItemNumber { get; set; }
        [MaxLength(6)]
        public string CustomerNumber { get; set; }
        [MaxLength(3)]
        public string BranchId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(25)]
        public string Brand { get; set; }
        [MaxLength(30)]
        public string Supplier { get; set; }
        [MaxLength(4)]
        public string Pack { get; set; }
        [MaxLength(8)]
        public string Size { get; set; }
        [MaxLength(6)]
        public string Vendor { get; set; }
        public bool Each { get; set; }
        public decimal CasePrice { get; set; }
        public decimal PackagePrice { get; set; }
        public string Label { get; set; }
    }
}
