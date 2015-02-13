using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
	[DataContract]
	[Serializable]
	public class UserSelectedContext
	{
		[DataMember(Name="customerid")]
		public string CustomerId { get; set; }
		[DataMember(Name="branchid")]
		public string BranchId { get; set; }
	}
}
