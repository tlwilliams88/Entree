using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class PasswordResetRequest : BaseEFModel
	{
		public Guid UserId { get; set; }
		[MaxLength(300)]
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
		public bool Processed { get; set; }
	}
}
