using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract]
	public class QuickAddReturnModel
	{
		[DataMember(Name = "success")]
		public bool Success { get; set; }

		[DataMember(Name = "id")]
		public Guid CartId { get; set; }

		[DataMember(Name = "errormessage")]
		public string ErrorMessage { get; set; }
	}
}
