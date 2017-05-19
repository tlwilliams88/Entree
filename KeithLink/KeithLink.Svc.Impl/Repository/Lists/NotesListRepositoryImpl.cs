using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class NotesListRepositoryImpl : DapperDatabaseConnection, INotesListRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetNotesHeaderByCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadNotesDetailsByParentId]";
        #endregion
        #region constructor
        public NotesListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ListModel> GetNotesList(UserSelectedContext catalogInfo, bool headerOnly)
        {
            NotesListHeader header = ReadOne<NotesListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));

            if (headerOnly == false)
            {
                header.Items = Read<NotesListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentNotesHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            return new List<ListModel>() { header.ToListModel(catalogInfo) };
        }

        #endregion
    }
}
