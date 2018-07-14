// KeithLink
using Entree.Core.Interface.Lists;
using Entree.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

// Core
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
