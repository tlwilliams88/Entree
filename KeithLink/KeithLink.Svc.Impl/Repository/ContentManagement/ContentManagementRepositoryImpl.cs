using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ContentManagement.EF;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ContentManagementItemRepositoryImpl : EFBaseRepository<ContentItem>, IContentManagementItemRepository
	{
        public ContentManagementItemRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IEnumerable<ContentItem> ReadActiveContentItemsByBranch(string branchId, int count)
        {
            return this.Entities.Where(c => c.BranchId.Equals(branchId, StringComparison.CurrentCultureIgnoreCase)
                && c.ActiveDateStart <= DateTime.UtcNow && c.ActiveDateEnd >= DateTime.UtcNow).OrderByDescending(c => c.ActiveDateEnd).Take(count).ToList();
        }

        public IEnumerable<ContentItem> ReadContentItemsByBranch(string branchId, int count)
        {
            return this.Entities.Where(c => c.BranchId.Equals(branchId, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(c => c.CreatedUtc).Take(count).ToList();
        }
    }
}
