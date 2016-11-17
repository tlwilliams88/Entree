// KeithLink
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;

using KeithLink.Svc.Impl.Repository.DataConnection;
using KeithLink.Svc.Impl.Repository.EF.Operational;

// Plugins
using Autofac;
using Dapper;

//Core
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class CustomInventoryItemsRepositoryImpl : EFBaseRepository<CustomInventoryItem>, ICustomInventoryItemsRepository  {
        #region attributes
        DapperDatabaseConnection _connection;

        private const string SP_DELETE_CUSTOM_INVENTORY_ITEMS = "List.DeleteCustomInventoryItems";
        private const string TVP_BIG_INT_LIST = "BigIntList";
        #endregion

        #region constructor
        public CustomInventoryItemsRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) {
            _connection = new DataConnection.DapperDatabaseConnection(Configuration.BEKDBConnectionString);
        }
        #endregion

        #region methods

        #region Retrieve
        public CustomInventoryItem Get(long id) {
            return this.Entities.Where(x => x.Id == id).FirstOrDefault();
        }

        public CustomInventoryItem Get(string branchId, string customerNumber, string itemNumber) {
            return this.Entities.Where(x => 
                            x.BranchId == branchId &&
                            x.CustomerNumber == customerNumber &&
                            x.ItemNumber == itemNumber
                        ).FirstOrDefault();
        }

        public List<CustomInventoryItem> GetItemsByItemIds(List<long> items) {
            return this.Entities.Where(x => items.Contains(x.Id)).ToList<CustomInventoryItem>();
        }

        public List<CustomInventoryItem> GetItemsByBranchAndCustomer(string branchId, string customerNumber) {
            return this.Entities.Where(x => 
                            x.BranchId == branchId && 
                            x.CustomerNumber == customerNumber
                        ).ToList<CustomInventoryItem>();
        }
        #endregion

        #region Saves
        public void Save(CustomInventoryItem item) {
            this.Entities.Add(item);
            this.UnitOfWork.SaveChanges();
        }

        public void SaveRange(List<CustomInventoryItem> items) {
            this.Entities.AddRange(items);

            // Set added or modified state based on Id
            foreach (CustomInventoryItem item in items) {
                this.UnitOfWork.Context.Entry(item).State = item.Id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
            }

            this.UnitOfWork.SaveChanges();
        }
        #endregion

        #region Deletes
        public void Delete(long id) {
            _connection.ExecuteCommand(GetDeleteCommandWithTvpParameters(new List<long> { id }));
        }

        public void DeleteRange(List<long> ids)
        {
            _connection.ExecuteCommand(GetDeleteCommandWithTvpParameters(ids));
        }

        public void DeleteRange(List<CustomInventoryItem> items) {
            List<long> KeysToDelete = items.Select(x => x.Id).ToList<long>();
            
            _connection.ExecuteCommand(GetDeleteCommandWithTvpParameters(KeysToDelete));
        }
        #endregion

        #region helpers
        private CommandDefinition GetDeleteCommandWithTvpParameters(List<long> ids)
        {
            DataTable parameterTable = new DataTable()
            {
                Columns = { { "Id", typeof(long) } },
            };

            foreach (long id in ids)
            {
                parameterTable.Rows.Add(id);
            }

            CommandDefinition command = new CommandDefinition(SP_DELETE_CUSTOM_INVENTORY_ITEMS,
                            new { Ids = parameterTable.AsTableValuedParameter(TVP_BIG_INT_LIST) },
                commandType: System.Data.CommandType.StoredProcedure);

            return command;
        }
        #endregion

        #endregion
    }
}
