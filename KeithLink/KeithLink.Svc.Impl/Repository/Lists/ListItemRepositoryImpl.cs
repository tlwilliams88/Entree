using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ListItemRepositoryImpl: EFBaseRepository<ListItem>, IListItemRepository
	{
		public ListItemRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
	}
}
