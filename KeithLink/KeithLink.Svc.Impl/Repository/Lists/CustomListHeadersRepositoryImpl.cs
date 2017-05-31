using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.DataConnection;
using System.Data;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListHeadersRepositoryImpl : DapperDatabaseConnection, ICustomListHeadersRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetCustomListHeaderById]";
        private const string COMMAND_GETHEADERS = "[List].[GetCustomListHeadersByCustomerNumberBranch]";
        #endregion
        #region constructor
        public CustomListHeadersRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public CustomListHeader GetCustomListHeader(long Id)
        {
            return ReadOne<CustomListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @Id = Id },
                                commandType: CommandType.StoredProcedure
                            ));
        }

        public List<CustomListHeader> GetCustomListHeaders(UserSelectedContext catalogInfo)
        {
            return Read<CustomListHeader>(new CommandDefinition(
                       COMMAND_GETHEADERS,
                       new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                       commandType: CommandType.StoredProcedure
                   ));
        }
        #endregion
    }
}
