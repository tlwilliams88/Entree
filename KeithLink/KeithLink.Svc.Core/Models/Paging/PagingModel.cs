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
		[DataMember(Name = "sort")]
		public List<SortInfo> Sort { get; set; }
		[DataMember(Name = "filter")]
		public FilterInfo Filter { get; set; }
		[DataMember(Name = "terms")]
		public string Terms { get; set; }
	}

	

	

	
	
	
}
