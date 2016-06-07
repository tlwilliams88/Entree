using KeithLink.Svc.Core.Models.SiteCatalog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract(Name = "recentlist")]
	public class RecentNonBEKList
    {
		[DataMember(Name = "catalog")]
		public string Catalog { get; set; }
		[DataMember(Name = "items")]
		public List<RecentNonBEKItem> Items { get; set; }
		[JsonIgnore]
		public DateTime ModifiedOn { get; set; }
	}
}
