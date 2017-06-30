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
        private const string PARMNAME_HEADERID = "HeaderId";
        private const string PARMNAME_ITEMNUMBER = "ItemNumber";
        private const string PARMNAME_EACH = "Each";
        private const string PARMNAME_CATALOGID = "CatalogId";
        private const string PARMNAME_NOTE = "Note";
        private const string PARMNAME_LINENUMBER = "LineNumber";


        private const string PARMNAME_RETURNVALUE = "ReturnValue";
            

        private const string SPNAME_GET = "[List].[ReadNoteDetailByParentIdAndItemNumber]";
        private const string SPNAME_GETALL = "[List].[ReadNotesDetailsByParentId]";
        private const string SPNAME_SAVE = "[List].[SaveNotesByCustomerNumberBranch]";
        #endregion

        #region methods
        public NotesListDetail Get(long parentHeaderId, string itemNumber) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_HEADERID, parentHeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, itemNumber);

            return ReadOne<NotesListDetail>(SPNAME_GET, parms);
        }

        public List<NotesListDetail> GetAll(long parentHeaderId) {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_HEADERID, parentHeaderId);

            return Read<NotesListDetail>(SPNAME_GETALL, parms);
        }
        
        public long Save(NotesListDetail detail) { 
            DynamicParameters parms = new DynamicParameters();
            parms.Add(PARMNAME_ID, detail.Id);
            parms.Add(PARMNAME_HEADERID, detail.HeaderId);
            parms.Add(PARMNAME_ITEMNUMBER, detail.ItemNumber);
            parms.Add(PARMNAME_EACH, detail.Each);
            parms.Add(PARMNAME_CATALOGID, detail.CatalogId);
            parms.Add(PARMNAME_NOTE, detail.Note);
            parms.Add(PARMNAME_LINENUMBER, detail.LineNumber);

            parms.Add(PARMNAME_RETURNVALUE, direction:ParameterDirection.Output);

            ExecuteCommand(SPNAME_SAVE, parms);

            return parms.Get<long>(PARMNAME_RETURNVALUE);
        }
        #endregion
    }
}