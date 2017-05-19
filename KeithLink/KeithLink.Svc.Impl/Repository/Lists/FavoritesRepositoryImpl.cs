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
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class FavoritesRepositoryImpl : DapperDatabaseConnection, IFavoritesRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetFavoritesHeaderByUserIdCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadFavoritesDetailsByParentId]";
        #endregion
        #region constructor
        public FavoritesRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ListModel> GetFavoritesList(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            FavoritesListHeader header = ReadOne<FavoritesListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @UserId = userId, @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));

            if (headerOnly == false)
            {
                header.Items = Read<FavoritesListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentFavoritesHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            return new List<ListModel>() { header.ToListModel(catalogInfo) };
        }

        #endregion
    }
}
