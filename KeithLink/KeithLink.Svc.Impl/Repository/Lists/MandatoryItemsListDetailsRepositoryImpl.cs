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
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class MandatoryItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IMandatoryItemsListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_PARENT_MANDATORY_ITEMS_HEADER_ID = "HeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOG_ID = "CatalogId";
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_LINENUMBER = "LineNumber";
        private const string PARMNAME_QUANTITY = "Quantity";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GET = "[List].[ReadMandatoryItemDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[SaveMandatoryItemByCustomerNumberBranch]";
        private const string SPNAME_DELETE = "[List].[DeleteMandatoryItemDetails]";
        #endregion

        #region constructor
        public MandatoryItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public List<MandatoryItemsListDetail> GetAllByHeader(long parentHeaderId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_PARENT_MANDATORY_ITEMS_HEADER_ID, parentHeaderId);

            return Read<MandatoryItemsListDetail>(SPNAME_GET, parms);
        }

        public long Save(MandatoryItemsListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_PARENT_MANDATORY_ITEMS_HEADER_ID, model.HeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, model.ItemNumber);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_CATALOG_ID, model.CatalogId);
            parms.Add(PARMNAME_ACTIVE, true);
            parms.Add(PARMNAME_LINENUMBER, model.LineNumber);
            parms.Add(PARMNAME_QUANTITY, model.Quantity);

            parms.Add(PARMNAME_RETURNVALUE, direction: ParameterDirection.Output, dbType: DbType.Int64);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }

        public void Delete(long id) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, id);

            ExecuteCommand(SPNAME_DELETE, parms);
        }
        #endregion
    }
}
