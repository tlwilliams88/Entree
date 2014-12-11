using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract]
	public class UserActiveCartModel
	{
		[DataMember(Name = "userid")]
		public Guid UserId { get; set; }
		[DataMember(Name = "cartid")]
		public Guid CartId { get; set; }
	}
}
