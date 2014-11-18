using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Paging
{
	[DataContract]
	public class PagingModel
	{
		[DataMember(Name = "size")]
		public int? Size { get; set; }
		[DataMember(Name = "from")]
		public int? From { get; set; }
	}
}
