using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ModelExport
{
	[DataContract]
	[Serializable]
	public class ExportModelConfiguration
	{
		[DataMember(Name = "field")]
		public string Field { get; set; }
		[DataMember(Name = "label")]
		public string Label { get; set; }
		[DataMember(Name = "order")]
		public int Order { get; set; }
		[DataMember(Name = "selected")]
		public bool Selected { get; set; }
	}
}
