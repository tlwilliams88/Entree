using Entree.Core.Lists.Models.SiteCatalog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Lists.Models
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
