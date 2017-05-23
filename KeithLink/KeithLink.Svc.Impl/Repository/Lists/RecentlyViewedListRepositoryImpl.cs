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
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class RecentlyViewedListRepositoryImpl : DapperDatabaseConnection, IRecentlyViewedListRepository
    {
        #region attributes
        private const string COMMAND_GETHEADER = "[List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]";
        private const string COMMAND_GETDETAILS = "[List].[ReadRecentlyViewedDetailsByParentId]";
        private const string COMMAND_ADDFAVORITE = "[List].[AddOrUpdateRecentlyViewedByUserIdCustomerNumberBranch]";
        #endregion
        #region constructor
        public RecentlyViewedListRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {

        }
        #endregion
        #region methods
        public List<ListModel> GetRecentlyViewedList(string userId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            RecentlyViewedListHeader header = ReadOne<RecentlyViewedListHeader>(new CommandDefinition(
                                COMMAND_GETHEADER,
                                new { @UserId = userId, @CustomerNumber = catalogInfo.CustomerId, @BranchId = catalogInfo.BranchId },
                                commandType: CommandType.StoredProcedure
                            ));

            if (headerOnly == false)
            {
                header.Items = Read<RecentlyViewedListDetail>(new CommandDefinition(
                                    COMMAND_GETDETAILS,
                                    new { @ParentRecentlyViewedHeaderId = header.Id },
                                    commandType: CommandType.StoredProcedure
                                ));
            }

            return new List<ListModel>() { header.ToListModel(catalogInfo) };
        }

        public void AddOrUpdateRecentlyViewed(string userId,
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
