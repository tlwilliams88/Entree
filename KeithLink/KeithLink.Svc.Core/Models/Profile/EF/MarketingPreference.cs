using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile.EF
{
	public class MarketingPreference: BaseEFModel
	{
		[MaxLength(150)]
		public string Email { get; set; }
		public string BranchId { get; set; }
		public bool CurrentCustomer { get; set; }
		public bool LearnMore { get; set; }
		public DateTime RegisteredOn { get; set; }
	}
}
