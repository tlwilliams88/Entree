using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{

	public class BranchSupport : BaseEFModel
	{
		public string BranchName { get; set; }
		public string BranchId { get; set; }
		public string SupportPhoneNumber { get; set; }
		public string TollFreeNumber { get; set; }
		public string Email { get; set; }
	}
}
