using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class ListShare:BaseEFModel
	{
		public string CustomerId { get; set; }
		public string BranchId { get; set; }
		public List SharedList { get; set; }
	}
}
