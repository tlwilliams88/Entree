using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListItemReportModel
	{
		[DataMember]
		public string ItemNumber { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string PackSize { get; set; }
		[DataMember]
		public int InCart { get; set; }
		[DataMember]
		public string Brand { get; set; }
	}
}
