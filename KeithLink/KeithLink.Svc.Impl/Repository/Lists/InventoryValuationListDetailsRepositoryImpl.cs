using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Amazon.CognitoIdentity.Model;
using Amazon.ElasticLoadBalancing.Model.Internal.MarshallTransformations;
using CommerceServer.Core.Inventory;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class InventoryValuationListDetailsRepositoryImpl : DapperDatabaseConnection, IInventoryValuationListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_CATALOG = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_LINENUMBER = "LineNumber";
        private const string PARMNAME_INVID = "CustomInventoryItemid";
        private const string PARMNAME_QTY = "Quantity";

        private const string PARMNAME_RETURNVALUE = "ReturnValue";

        private const string SPNAME_GETDETAILS = "[List].[ReadInventoryValuationListDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[SaveInventoryValuationItem]";
        #endregion

        #region constructor
        public InventoryValuationListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        #region methods
        public List<InventoryValuationListDetail> GetInventoryValuationDetails(long headerId) {
            return Read<InventoryValuationListDetail>(SPNAME_GETDETAILS, PARMNAME_HEADERID, headerId);
        }

        public long SaveInventoryValuationDetail(InventoryValuationListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOG, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_ITEMNUMBER, model.ItemNumber);
            parms.Add(PARMNAME_LINENUMBER, model.LineNumber);
            parms.Add(PARMNAME_INVID, model.CustomInventoryItemId);
            parms.Add(PARMNAME_QTY, model.Quantity);

            parms.Add(PARMNAME_RETURNVALUE, direction: ParameterDirection.Output, dbType: DbType.Int64);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }
        #endregion
    }
}
