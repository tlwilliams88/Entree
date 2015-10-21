﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Impl.Logging;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Repository.EF.Operational {
	public class UnitOfWork : IUnitOfWork {
		private IEventLogRepository eventLogRepository;
		
		#region ctor
		public UnitOfWork(IEventLogRepository eventLogRepository)
		{
			this.Context = new BEKDBContext(eventLogRepository);
            this.Context.Configuration.LazyLoadingEnabled = false;
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
