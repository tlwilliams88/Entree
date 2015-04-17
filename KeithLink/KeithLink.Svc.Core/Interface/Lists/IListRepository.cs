using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
	public interface IListRepository : IBaseEFREpository<List>
	{
		IQueryable<List> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly);
		
	}
}
