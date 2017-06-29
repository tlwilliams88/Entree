using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Linq;

namespace KeithLink.Svc.Core.Interface.Lists
{
	public interface IListRepository : IBaseEFREpository<List>
	{
		IQueryable<List> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly);
		
	}
}
