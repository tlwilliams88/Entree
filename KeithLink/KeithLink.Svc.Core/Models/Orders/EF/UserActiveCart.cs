using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders.EF
{
	public class UserActiveCart: BaseEFModel
	{
		public Guid UserId { get; set; }
		public Guid CartId { get; set; }
		public string CustomerId { get; set; }
		public string BranchId { get; set; }
	}
}
