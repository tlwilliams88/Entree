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
	public class ListShareRepositoryImpl: EFBaseRepository<ListShare>, IListShareRepository
	{
		public ListShareRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
	}
}
