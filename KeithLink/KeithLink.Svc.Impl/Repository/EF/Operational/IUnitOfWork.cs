using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
	public interface IUnitOfWork
	{
		BEKDBContext Context { get; }
		int SaveChanges();
	}
}
