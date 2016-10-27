using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class ListItem: BaseEFModel
	{
        [Required]
		[MaxLength(15)]
		public string ItemNumber { get; set; }
		[MaxLength(150)]
		public string Label { get; set; }
		public decimal Par { get; set; }
		[MaxLength(200)]
		public string Note { get; set; }
		[MaxLength(40)]
		public string Category { get; set; }
		public int Position { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public List ParentList { get; set; }
		public bool? Each { get; set; }
		public decimal Quantity { get; set; }
        [MaxLength(24)]
        public string CatalogId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(4)]
        public string Pack { get; set; }
        [MaxLength(8)]
        public string Size { get; set; }
        [MaxLength(6)]
        public string Vendor { get; set; }
        [MaxLength(10)]
        public string CasePrice { get; set; }
        [MaxLength(10)]
        public string PackagePrice { get; set; }
    }
}
