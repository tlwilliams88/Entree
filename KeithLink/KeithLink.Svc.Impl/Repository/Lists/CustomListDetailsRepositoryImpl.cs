using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListDetailsRepositoryImpl : DapperDatabaseConnection, ICustomListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_CATALOG = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_INVID = "CustomInventoryItemId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_LINENUMBER = "LineNumber";
        private const string PARMNAME_LABEL = "Label";
        private const string PARMNAME_PAR = "Par";

        private const string PARMNAME_RETVAL = "ReturnValue";

        private const string SPNAME_GETBYHEADER = "[List].[ReadCustomListDetailsByParentId]";
        private const string SPNAME_SAVE= "[List].[SaveCustomListDetails]";
        #endregion

        #region constructor
        public CustomListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public List<CustomListDetail> GetCustomListDetails(long headerId) {
            return Read<CustomListDetail>(SPNAME_GETBYHEADER, PARMNAME_HEADERID, headerId);
        }

        public long SaveCustomListDetail(CustomListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOG, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_INVID, model.CustomInventoryItemId);
            parms.Add(PARMNAME_ITEMNUMBER, model.ItemNumber);
            parms.Add(PARMNAME_LINENUMBER, model.LineNumber);
            parms.Add(PARMNAME_LABEL, model.Label);
            parms.Add(PARMNAME_PAR, model.Par);
            parms.Add(PARMNAME_RETVAL, dbType: DbType.Int64, direction: ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETVAL);
        }
        #endregion
    }
}
