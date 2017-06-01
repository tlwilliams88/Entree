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
using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class CustomListSharesRepositoryImpl : DapperDatabaseConnection, ICustomListSharesRepository
    {
        #region attributes
        private const string COMMAND_GETHEADERS = "[List].[GetCustomListSharesByListId]";
        private const string COMMAND_GETHEADERS2 = "[List].[GetCustomListSharesByCustomerNumberBranch]";
        #endregion
        #region constructor
        public CustomListSharesRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<CustomListShare> GetCustomListShares(UserSelectedContext catalogInfo)
        {
            return Read<CustomListShare>(new CommandDefinition(
                                COMMAND_GETHEADERS2,
                                new { @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));
        }

        public List<CustomListShare> GetCustomListShares(long Id)
        {
            return Read<CustomListShare>(new CommandDefinition(
                       COMMAND_GETHEADERS,
                       new { @ListId = Id },
                       commandType: CommandType.StoredProcedure
                   ));
        }
        #endregion
    }
}
