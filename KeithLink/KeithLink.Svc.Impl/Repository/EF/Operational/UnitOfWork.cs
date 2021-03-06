﻿using KeithLink.Common.Core.Interfaces.Logging;

using System.Data.Common;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
	public class UnitOfWork : IUnitOfWork {
		#region ctor
		public UnitOfWork(IEventLogRepository eventLogRepository)
		{
			this.Context = new BEKDBContext(eventLogRepository);
            this.Context.Configuration.LazyLoadingEnabled = true;
        }

        public UnitOfWork(string nameOrConnectionString) {
            if (string.IsNullOrEmpty(nameOrConnectionString))
                this.Context = new BEKDBContext();
            else
                this.Context = new BEKDBContext(nameOrConnectionString);
        }

        public UnitOfWork(DbConnection existingConnection) {
            if (existingConnection == null)
                this.Context = new BEKDBContext();
            else
                this.Context = new BEKDBContext(existingConnection);
        }
        #endregion

        #region methods
        public void ClearContext() {
            this.Context.UndoDBContextChanges();
        }

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
