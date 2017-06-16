using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecommendedItemsListHeadersRepositoryImpl : DapperDatabaseConnection, IRecommendedItemsListHeadersRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetRecentlyOrderedHeaderByUserIdCustomerNumberBranch]";
        #endregion
        #region constructor
        public RecommendedItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public RecommendedItemsListHeader GetRecommendedItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return ReadOne<RecommendedItemsListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @UserId = userId, @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));
        }
        #endregion
    }
}
