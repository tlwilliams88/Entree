using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.EF.Operational
{
	public class UnitOfWork : IUnitOfWork
	{
		public BEKDBContext Context { get; private set; }

		public UnitOfWork()
		{
			this.Context = new BEKDBContext();
			this.Context.Configuration.LazyLoadingEnabled = true;
		}

		public UnitOfWork(string nameOrConnectionString)
		{
			if (string.IsNullOrEmpty(nameOrConnectionString))
				this.Context = new BEKDBContext();
			else
				this.Context = new BEKDBContext(nameOrConnectionString);
		}

		public UnitOfWork(DbConnection existingConnection)
		{
			if (existingConnection == null)
				this.Context = new BEKDBContext();
			else
				this.Context = new BEKDBContext(existingConnection);
		}

		public int SaveChanges()
		{
			return this.Context.SaveChanges();
		}
	}
}
