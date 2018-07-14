using Entree.Core.Models.EF;
using Entree.Core.Models.SiteCatalog;
using System.Linq;

namespace Entree.Core.Interface.Lists
{
	public interface IListRepository : IBaseEFREpository<List>
	{
		IQueryable<List> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly);
		
	}
}
