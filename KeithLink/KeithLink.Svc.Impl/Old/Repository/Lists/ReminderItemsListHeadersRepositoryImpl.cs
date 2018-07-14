﻿using System.Data;

using Dapper;

using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists.ReminderItems;
using Entree.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class ReminderItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IRemindersListHeadersRepository {
        #region attributes
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_RETVAL = "ReturnValue";

        private const string SPNAME_GETONE = "[List].[GetRemindersHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveRemindersHeaders]";
        #endregion

        #region constructor
        public ReminderItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public ReminderItemsListHeader GetReminderItemsHeader(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOneSP<ReminderItemsListHeader>(SPNAME_GETONE, parms);
        }

        public long SaveReminderListHeader(ReminderItemsListHeader model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_RETVAL, 0, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteSPCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }
        #endregion
    }
}
