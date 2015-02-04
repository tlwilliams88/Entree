using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Impl.Logging;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
	public class UnitOfWork : IUnitOfWork {
        #region ctor
        public UnitOfWork() {
            this.Context = new BEKDBContext(new EventLogRepositoryImpl(Configuration.ApplicationName));
            this.Context.Configuration.LazyLoadingEnabled = true;
        }

        public UnitOfWork(string nameOrConnectionString) {
            if (string.IsNullOrEmpty(nameOrConnectionString))
                this.Context = new BEKDBContext(new EventLogRepositoryImpl(Configuration.ApplicationName));
            else
                this.Context = new BEKDBContext(nameOrConnectionString);
        }

        public UnitOfWork(DbConnection existingConnection) {
            if (existingConnection == null)
                this.Context = new BEKDBContext(new EventLogRepositoryImpl(Configuration.ApplicationName));
            else
                this.Context = new BEKDBContext(existingConnection);
        }
        #endregion

        #region methods
        public IUnitOfWork GetUniqueUnitOfWork() {
            return new UnitOfWork(Context.Database.Connection);
        }

        public int SaveChanges() {
            return this.Context.SaveChanges();
        }


        public int SaveChangesAndClearContext() {
            var returnValue = this.SaveChanges();
            this.Context.UndoDBContextChanges();
            return returnValue;
        }
        #endregion

        #region properties
        public BEKDBContext Context { get; private set; }
        #endregion
    }
}
