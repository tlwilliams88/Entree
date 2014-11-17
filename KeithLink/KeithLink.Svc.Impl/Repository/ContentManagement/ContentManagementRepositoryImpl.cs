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
    }
}
