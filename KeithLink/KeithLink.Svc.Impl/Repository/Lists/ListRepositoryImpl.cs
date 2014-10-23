using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.EF;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ListRepositoryImpl : EFBaseRepository<List>, IListRepository
	{
		public ListRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        

		public IEnumerable<List> ReadListForCustomer(Core.Models.Profile.UserProfile user, Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly)
		{
			if(headerOnly)
				return this.Entities.Where(l => (l.UserId.Equals(user.UserId) && l.CustomerId.Equals(catalogInfo.CustomerId)) || (l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)));
			else
				return this.Entities.Include(i => i.Items).Where(l => (l.UserId.Equals(user.UserId) && l.CustomerId.Equals(catalogInfo.CustomerId)) || (l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)));
		}



		public IEnumerable<List> ReadListForUser(Core.Models.Profile.UserProfile user, Core.Models.SiteCatalog.UserSelectedContext catalogInfo,  bool headerOnly)
		{
			if (headerOnly)
				return this.Entities.Where(l => l.UserId.Equals(user.UserId) && (l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)));
			else
				return this.Entities.Include(i => i.Items).Where(l => l.UserId.Equals(user.UserId) && (l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)));
		}
	}
}
