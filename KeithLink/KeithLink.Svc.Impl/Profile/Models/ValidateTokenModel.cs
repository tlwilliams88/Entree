using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models
{
	[DataContract]
	public class ValidateTokenModel
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }
	}
}
