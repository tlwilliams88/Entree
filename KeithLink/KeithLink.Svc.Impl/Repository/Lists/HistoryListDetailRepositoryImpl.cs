using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class HistoryListDetailRepositoryImpl : DapperDatabaseConnection, IHistoryListDetailRepository {
        #region attributes
        private const string PARMNAME_LISTID = "HeaderId";

        private const string SPNAME_GETALL = "[List].[ReadHistoryDetailsByParentId]";
        #endregion

        #region ctor
        public HistoryListDetailRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods
        public List<HistoryListDetail> GetAllHistoryDetails(long listId) {
            return Read<HistoryListDetail>(SPNAME_GETALL, PARMNAME_LISTID, listId);
        }
        #endregion
    }
}
