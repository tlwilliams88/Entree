using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using System.Data.Entity.Infrastructure;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class HistoryListRepositoryImpl : DapperDatabaseConnection, IHistoryListRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetHistoryHeaderByCustomerNumberAndBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadHistoryDetailsByParentId]";

        #endregion
        #region constructor
        public HistoryListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        public List<ListModel> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly)
        {
            var header = ReadOne<HistoryListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));

            if (headerOnly == false)
            {
                header.Items = Read<HistoryListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentHistoryHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            List <ListModel> list = new List<ListModel>() { header.ToListModel(catalogInfo) };
            return list;
        }
    }
}
