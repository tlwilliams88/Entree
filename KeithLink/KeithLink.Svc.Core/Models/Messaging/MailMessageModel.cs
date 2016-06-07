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
        [DataMember(Name = "isalert")]
        public bool IsAlert { get; set; }
        [DataMember(Name = "branchestoalert")]
        public string BranchesToAlert { get; set; }
        [DataMember(Name = "customers")]
		public List<Guid> CustomerIds { get; set; }
		[DataMember(Name = "users")]
		public List<Guid> UserIds { get; set; }
		[DataMember(Name = "msg")]
		public UserMessageModel Message { get; set; }
	}
}
