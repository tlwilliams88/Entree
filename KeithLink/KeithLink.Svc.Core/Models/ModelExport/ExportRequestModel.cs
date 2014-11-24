using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ModelExport
{
	[DataContract]
	public class ExportRequestModel
	{
		[DataMember(Name="fields")]
		public List<ExportModelConfiguration> Fields { get; set; }
		[DataMember(Name = "selectedtype")]
		public string SelectedType { get; set; }
	}
}
