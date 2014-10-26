using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class ListItem: BaseEFModel
	{
		[Required]
		public string ItemNumber { get; set; }
		public string Label { get; set; }
		public decimal Par { get; set; }
		public string Note { get; set; }
		public string Category { get; set; }
		public int Position { get; set; }

		public List ParentList { get; set; }
	}
}
