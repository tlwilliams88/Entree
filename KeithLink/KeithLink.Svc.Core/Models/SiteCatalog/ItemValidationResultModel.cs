using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.SiteCatalog
{
	[DataContract]
	public class ItemValidationResultModel
	{
		[DataMember(Name="itemnumber")]
		public string ItemNumber { get; set; }
		[DataMember(Name="valid")]
		public bool Valid { get; set; }
		[DataMember(Name="reason")]
		public InvalidReason Reason { get; set; }
	}

	public enum InvalidReason
	{
		InvalidItemNumber,
		EachNotAllowed
	}
}
