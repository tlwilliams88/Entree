using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
	[DataContract]
	public class ItemNote
	{
		[DataMember(Name="itemnumber")]
		public string ItemNumber { get; set; }
		[DataMember(Name = "note")]
		public string Note { get; set; }
	}
}
