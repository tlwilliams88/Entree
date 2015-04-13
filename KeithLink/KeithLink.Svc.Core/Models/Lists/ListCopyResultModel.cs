using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListCopyResultModel
	{
		[DataMember(Name = "customerid")]
		public string CustomerId { get; set; }
		[DataMember(Name = "branchid")]
		public string BranchId { get; set; }
		[DataMember(Name = "newlistid")]
		public long NewListId { get; set; }
	}
}
