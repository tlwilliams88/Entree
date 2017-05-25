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
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecentlyOrderedListHeadersRepositoryImpl : DapperDatabaseConnection, IRecentlyOrderedListHeadersRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetRecentlyOrderedHeaderByUserIdCustomerNumberBranch]";
        #endregion
        #region constructor
        public RecentlyOrderedListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public RecentlyOrderedListHeader GetRecentlyOrderedHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return ReadOne<RecentlyOrderedListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @UserId = userId, @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));
        }
        #endregion
    }
}
