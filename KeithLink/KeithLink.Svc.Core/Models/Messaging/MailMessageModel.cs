using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging
{
	[DataContract]
	public class MailMessageModel
	{
		[DataMember(Name = "customers")]
		public List<Guid> CustomerIds { get; set; }
		[DataMember(Name = "users")]
		public List<Guid> UserIds { get; set; }
		[DataMember(Name = "message")]
		public UserMessageModel Message { get; set; }
	}
}
