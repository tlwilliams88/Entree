using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
	public interface IUnitOfWork
	{
        void ClearContext();
		BEKDBContext Context { get; }
        IUnitOfWork GetUniqueUnitOfWork();
		int SaveChanges();
		int SaveChangesAndClearContext();
		
	}
}
