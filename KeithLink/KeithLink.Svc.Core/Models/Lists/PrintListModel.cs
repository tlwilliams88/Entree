using KeithLink.Svc.Core.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class PrintListModel
	{
		[DataMember(Name="landscape")]
		public bool Landscape { get; set; }
		[DataMember(Name = "paging")]
		public PagingModel Paging { get; set; }
        [DataMember(Name = "showparvalues")]
        public bool ShowParValues { get; set; }
	}
}
