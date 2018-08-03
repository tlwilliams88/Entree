using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Paging
{
    [DataContract]
	public class PagedResultsForCustomersIntegration<T>
    {
        [DataMember(Name = "totalResults")]
        public long TotalResults => Results.Count;

		[DataMember(Name = "results")]
		public List<T> Results { get; set; }
	}
}
