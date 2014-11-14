using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListCopyShareModel
	{
		[DataMember(Name = "listid")]
		public long ListId { get; set; }
		[DataMember(Name = "customers")]
		public List<Customer> Customers { get; set; }
	}
		
}
