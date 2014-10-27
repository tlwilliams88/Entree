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
	[DataContract(Name = "recentitem")]
	public class RecentItem
	{
		[DataMember(Name = "itemnumber")]
		public string ItemNumber { get; set; }
		[DataMember(Name = "images")]
		public List<ProductImage> Images { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[JsonIgnore]
		public DateTime ModifiedOn { get; set; }
	}
}
