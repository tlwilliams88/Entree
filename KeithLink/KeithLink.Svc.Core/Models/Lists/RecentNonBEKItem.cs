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
	public class RecentNonBEKItem
	{
		[DataMember(Name = "itemnumber")]
		public string ItemNumber { get; set; }
        [DataMember(Name = "catalogid")]
        public string CatalogId { get; set; }
        [DataMember(Name = "images")]
		public List<ProductImage> Images { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
        [DataMember(Name = "upc")]
        public string Upc { get; set; }
        [JsonIgnore]
		public DateTime ModifiedOn { get; set; }
	}
}
