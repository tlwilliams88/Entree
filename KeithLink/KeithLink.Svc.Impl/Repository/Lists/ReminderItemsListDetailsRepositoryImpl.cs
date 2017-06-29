using System.Collections.Generic;

using Dapper;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ReminderItemsListDetailsRepositoryImpl : DapperDatabaseConnection, IRemindersListDetailsRepository
    {
        #region attributes
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_CATALOG = "CatalogId";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_ITEMNUM = "ItemNumber";

        private const string SPNAME_DELETE = "[List].[DeleteReminderItemDetails]";
        private const string SPNAME_GETALL = "[List].[ReadReminderDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[SaveRemindersDetails]";
        #endregion
        
        #region constructor
        public ReminderItemsListDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion

        #region methods
        public void DeleteReminderListDetail(long id) {
            ExecuteCommand(SPNAME_DELETE, PARMNAME_ID, id);
        }

        public List<ReminderItemsListDetail> GetRemindersDetails(long parentHeaderId) {
            return Read<ReminderItemsListDetail>(SPNAME_GETALL, PARMNAME_HEADERID, parentHeaderId);
        }

        public void SaveReminderListDetail(ReminderItemsListDetail model) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ACTIVE, model.Active);
            parms.Add(PARMNAME_CATALOG, model.CatalogId);
            parms.Add(PARMNAME_EACH, model.Each);
            parms.Add(PARMNAME_HEADERID, model.HeaderId);
            parms.Add(PARMNAME_ID, model.Id);
            parms.Add(PARMNAME_ITEMNUM, model.ItemNumber);

            ExecuteCommand(SPNAME_SAVE, parms);
        }
        #endregion
    }
}
