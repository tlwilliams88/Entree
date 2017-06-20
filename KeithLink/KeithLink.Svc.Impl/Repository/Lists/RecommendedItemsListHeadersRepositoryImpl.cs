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
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_BRANCH = "BranchId";
        private const string PARMNAME_CUSTNUM = "CustomerNumber";
        private const string PARMNAME_RETVAL = "ReturnValue";
        private const string SPNAME_GETONE = "[List].[GetRecommendedItemsHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[SaveRecommendedItemHeaderByCustomerNumberBranch]";
        #endregion
        #region constructor
        public RecommendedItemsListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public RecommendedItemsListHeader GetRecommendedItemsHeaderByCustomerNumberBranch(UserSelectedContext catalogInfo)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_BRANCH, catalogInfo.BranchId);
            parms.Add(PARMNAME_CUSTNUM, catalogInfo.CustomerId);

            return ReadOne<RecommendedItemsListHeader>(SPNAME_GETONE, parms);
        }

        public long SaveRecommendedItemsHeader(RecommendedItemsListHeader model)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_BRANCH, model.BranchId);
            parms.Add(PARMNAME_CUSTNUM, model.CustomerNumber);
            parms.Add(PARMNAME_RETVAL, 0, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }
        #endregion
    }
}
