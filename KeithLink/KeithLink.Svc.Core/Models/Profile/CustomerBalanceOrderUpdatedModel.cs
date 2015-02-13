using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
	[DataContract]
	[Serializable]
	public class CustomerBalanceOrderUpdatedModel
	{
		[DataMember(Name = "balance")]
		public CustomerAccountBalanceModel balance { get; set; }

		[DataMember(Name = "lastorderupdate")]
		public DateTime? LastOrderUpdate { get; set; }
	}
}
