using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ReminderItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IRemindersListHeadersRepository
    {
        #region attributes
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";

        private const string SPNAME_GETONE = "[List].[GetRemindersHeaderByCustomerNumberBranch]";
        #endregion

        #region constructor
        public ReminderItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public ReminderItemsListHeader GetReminderItemsHeader(UserSelectedContext catalogInfo) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOne<ReminderItemsListHeader>(SPNAME_GETONE, parms);
        }
        #endregion

        
    }
}
