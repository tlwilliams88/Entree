using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using System.Web.UI.WebControls.WebParts;
using Amazon.CognitoIdentity.Model;
using CommerceServer.Core.Inventory;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListDetailsRepositoryImpl : DapperDatabaseConnection, ICustomListDetailsRepository
    {
        #region attributes

        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_CATALOG = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "ParentCustomListHeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_INVID = "CustomInventoryItemId";
        private const string PARMNAME_ITEMNUM = "ItemNumber";
        private const string PARMNAME_LABEL = "Label";
        private const string PARMNAME_PAR = "Par";

        private const string SPNAME_GETBYHEADER = "[List].[ReadCustomListDetailsByParentId]";
        private const string SPNAME_SAVE= "[List].[AddOrUpdateCustomListItemById]";
        #endregion

        #region constructor
        public CustomListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public List<CustomListDetail> GetCustomListDetails(long headerId) {
            return Read<CustomListDetail>(SPNAME_GETBYHEADER, PARMNAME_HEADERID, headerId);
        }

        public void SaveCustomListDetail(CustomListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOG, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_INVID, model.CustomInventoryItemId);
            parms.Add(PARMNAME_ITEMNUM, model.ItemNumber);
            parms.Add(PARMNAME_LABEL, model.Label);
            parms.Add(PARMNAME_PAR, model.Par);

            ExecuteCommand(SPNAME_SAVE, parms);
        }
        #endregion
    }
}
