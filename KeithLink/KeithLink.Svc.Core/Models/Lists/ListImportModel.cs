using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListImportModel
	{
		[DataMember(Name="listid")]
		public long? ListId { get; set; }
		[DataMember(Name="success")]
		public bool Success { get; set; }
		[DataMember(Name="errormsg")]
		public string ErrorMessage { get; set; }
		[DataMember(Name="warningmsg")]
		public string WarningMessage { get; set; }
	}
}
