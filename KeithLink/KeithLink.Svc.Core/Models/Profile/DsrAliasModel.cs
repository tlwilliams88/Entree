using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile {
	
	[DataContract]
	[Serializable]
    public class DsrAliasModel {
		[DataMember(Name = "id")]
        public long Id { get; set; }
		[DataMember(Name = "userid")]
        public Guid UserId { get; set; }
		[DataMember(Name = "email")]
        public string Email { get; set; }
		[DataMember(Name = "branchid")]
        public string BranchId { get; set; }
		[DataMember(Name = "dsrnumber")]
        public string DsrNumber { get; set; }
    }
}
