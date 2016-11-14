// KeithLink
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;


using KeithLink.Svc.Impl.Repository.EF.Operational;

// Plugins
using Autofac;

//Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists {
    public class CustomInventoryItemsRepositoryImpl : EFBaseRepository<CustomInventoryItem>, ICustomInventoryItemsRepository {
        #region attributes
        #endregion

        #region constructor
        public CustomInventoryItemsRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
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
            CustomInventoryItem objectToDelete = new CustomInventoryItem() { Id = id };
            this.Entities.Attach(objectToDelete);
            this.Entities.Remove(objectToDelete);

            this.UnitOfWork.SaveChanges();
        }

        public void DeleteRange(List<CustomInventoryItem> items) {
            foreach (CustomInventoryItem item in items) {
                this.Entities.Attach(item);
                this.Entities.Remove(item);
            }
            
            this.UnitOfWork.SaveChanges();
        }
        #endregion

        #endregion
    }
}
