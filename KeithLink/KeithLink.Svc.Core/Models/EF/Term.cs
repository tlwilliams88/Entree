using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class Term: BaseEFModel
	{
		[MaxLength(3)]
		public string BranchId { get; set; }
		public int TermCode { get; set; }
		[MaxLength(25)]
		public string Description { get; set; }
		public int Age1 { get; set; }
		public int Age2 { get; set; }
		public int Age3 { get; set; }
		public int Age4 { get; set; }
	}
}
