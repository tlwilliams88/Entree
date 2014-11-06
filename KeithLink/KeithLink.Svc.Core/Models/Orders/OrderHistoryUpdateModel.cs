using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract]
	public class OrderHistoryUpdateModel
	{
		[DataMember(Name="lastupdated")]
		public DateTime? LastUpdated { get; set; }
	}
}
