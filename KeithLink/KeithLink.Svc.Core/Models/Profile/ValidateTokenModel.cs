using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
	[DataContract]
	public class ValidateTokenModel
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }
	}
}
