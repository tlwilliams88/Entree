using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class NotesDetailsRepositoryImpl : DapperDatabaseConnection, INotesDetailsListRepository {
        #region constructor
        public NotesDetailsRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region attributes
        private const string PARMNAME_ID = "Id";
        private const string PARMNAME_PARENTNOTESHEADERID = "ParentNotesHeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_NOTE = "Note";
        private const string PARMNAME_ACTIVE = "Active";
        private const string PARMNAME_RETURNVALUE = "ReturnValue";
            

        private const string SPNAME_GET = "[List].[GetNotesHeaderByCustomerNumberBranch]";
        private const string SPNAME_SAVE = "[List].[AddOrUpdateNotesByCustomerNumberBranch]";
        #endregion

        #region methods
        public List<NotesListDetail> Get(long parentHeaderId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_PARENTNOTESHEADERID, parentHeaderId);

            return Read<NotesListDetail>(SPNAME_GET, parms);
        }

        public long Save(NotesListDetail detail) { 
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, detail.Id);
            parms.Add(PARMNAME_PARENTNOTESHEADERID, detail.ParentNotesHeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, detail.ItemNumber);
            parms.Add(PARMNAME_EACH, detail.Each);
            parms.Add(PARMNAME_CATALOGID, detail.CatalogId);
            parms.Add(PARMNAME_NOTE, detail.Note);
            parms.Add(PARMNAME_ACTIVE, detail.Active);
            parms.Add(PARMNAME_RETURNVALUE, direction:ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }
        #endregion
    }
}