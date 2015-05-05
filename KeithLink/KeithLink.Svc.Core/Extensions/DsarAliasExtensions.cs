using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
	public static class DsarAliasExtensions
	{
		public static DsrAliasModel ToModel(this DsrAlias d)
		{
			return new DsrAliasModel() { BranchId = d.BranchId, Id = d.Id, DsrNumber = d.DsrNumber, UserId = d.UserId, Email = d.UserName };
		}
	}
}
