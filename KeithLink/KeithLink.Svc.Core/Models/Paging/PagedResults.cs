using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Paging
{
	[DataContract]
	public class PagedResults<T>
	{
		[DataMember(Name = "totalResults")]
		public long TotalResults { get; set; }
		[DataMember(Name = "results")]
		public List<T> Results { get; set; }
	}
}
