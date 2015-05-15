using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
	[DataContract]
	public class MarketingPreferenceModel
	{
		[DataMember(Name = "id")]
		public long Id { get; set; }
		[DataMember(Name = "email")]
		public string Email { get; set; }
		[DataMember(Name = "branchid")]
		public string BranchId { get; set; }
		[DataMember(Name = "iscustomer")]
		public bool CurrentCustomer { get; set; }
		[DataMember(Name = "learnmore")]
		public bool LearnMore { get; set; }
		[DataMember(Name = "registeredon")]
		public DateTime RegisteredOn { get; set; }
	}
}

