using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListReportModel
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public List<ListItemReportModel> Items { get; set; }
	}
}
