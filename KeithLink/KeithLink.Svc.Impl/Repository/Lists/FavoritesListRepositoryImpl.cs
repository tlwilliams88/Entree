﻿using System;
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
    public class FavoritesListRepositoryImpl : DapperDatabaseConnection, IFavoritesListRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetFavoritesHeaderByUserIdCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadFavoritesDetailsByParentId]";
        private const string COMMAND_ADDFAVORITE = "[List].[AddOrUpdateFavoriteByUserIdCustomerNumberBranch]";
        #endregion
        #region constructor
        public FavoritesListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
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

            if (header != null && headerOnly == false)
            {
                header.Items = Read<FavoritesListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentFavoritesHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            if (header != null)
            {
                return new List<ListModel>() {header.ToListModel(catalogInfo)};
            }
            return null;
        }

        public void AddOrUpdateFavorite(string userId,
                                string customerNumber,
                                string branchId,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            ExecuteCommand(new CommandDefinition(COMMAND_ADDFAVORITE,
                new
                {
                    @UserId = userId,
                    @CustomerNumber = customerNumber,
                    @BranchId = branchId,
                    @ItemNumber = itemNumber,
                    @Each = each,
                    @CatalogId = catalogId,
                    @Active = active
                }, commandType: CommandType.StoredProcedure));   
        }
        #endregion
    }
}
