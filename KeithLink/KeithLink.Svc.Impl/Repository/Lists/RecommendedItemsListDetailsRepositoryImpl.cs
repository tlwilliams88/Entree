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
using KeithLink.Svc.Core.Models.Lists.RecommendedItem;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecommendedItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IRecommendedItemsListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_PARENT_HEADER_ID = "HeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOG_ID = "CatalogId";

        private const string SPNAME_GET = "[List].[ReadRecommendedItemDetailsByHeaderId]";
        private const string SPNAME_SAVE = "[List].[SaveRecommendedItemByCustomerNumberBranch]";
        private const string SPNAME_DELETE = "[List].[DeleteRecommendedItemDetails]";
        #endregion
        #region constructor
        public RecommendedItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<RecommendedItemsListDetail> GetAllByHeader(long parentHeaderId)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_PARENT_HEADER_ID, parentHeaderId);

            return Read<RecommendedItemsListDetail>(SPNAME_GET, parms);
        }

        public void Save(RecommendedItemsListDetail model)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_PARENT_HEADER_ID, model.HeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, model.ItemNumber);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_CATALOG_ID, model.CatalogId);

            ExecuteCommand(SPNAME_SAVE, parms);
        }

        public void Delete(long id)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, id);

            ExecuteCommand(SPNAME_DELETE, parms);
        }
        #endregion
    }
}
