using System.Collections.Generic;
using System.Data;

using Dapper;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class NotesDetailsRepositoryImpl : DapperDatabaseConnection, INoteDetailsListRepository {
        #region constructor
        public NotesHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetNotesHeaderByCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadNotesDetailsByParentId]";
        private const string COMMAND_ADDFAVORITE = "[List].[AddOrUpdateNotesByCustomerNumberBranch]";
        #endregion

        #region methods
        public List<ListModel> GetNotesList(UserSelectedContext catalogInfo, bool headerOnly) {
            NotesListHeader header = ReadOne<NotesListHeader>(new CommandDefinition(
                                                                                    COMMAND_GETHEADER,
                                                                                    new {CustomerNumber = catalogInfo.CustomerId, catalogInfo.BranchId},
                                                                                    commandType: CommandType.StoredProcedure
                                                                                   ));

            if (headerOnly == false)
                header.Items = Read<NotesListDetail>(new CommandDefinition(
                                                                           COMMAND_GETDETAILS,
                                                                           new {ParentNotesHeaderId = header.Id},
                                                                           commandType: CommandType.StoredProcedure
                                                                          ));

            return new List<ListModel> {header.ToListModel(catalogInfo)};
        }

        public void AddOrUpdateNote(string customerNumber,
                                    string branchId,
                                    string itemNumber,
                                    bool each,
                                    string catalogId,
                                    string note,
                                    bool active) {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDFAVORITE,
                                                 new {
                                                         CustomerNumber = customerNumber,
                                                         BranchId = branchId,
                                                         ItemNumber = itemNumber,
                                                         Each = each,
                                                         CatalogId = catalogId,
                                                         Note = note,
                                                         Active = active
                                                     }, commandType: CommandType.StoredProcedure));
        }
        #endregion
    }
}