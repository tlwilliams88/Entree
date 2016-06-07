using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Core.Interface.Lists;

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class HistoryLogic : IHistoryLogic {
        #region attributes
        private readonly IListRepository _repo;
        #endregion

        #region ctor
        public HistoryLogic(IListRepository listRepository) {
            _repo = listRepository;
        }
        #endregion

        #region methods
        public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers) {
            var returnModel = new BlockingCollection<InHistoryReturnModel>();

            var list = _repo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) && 
                                       l.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase) && 
                                       l.Type == ListType.Worksheet, 
                                  i => i.Items)
                            .FirstOrDefault();

            if(list == null)
                return itemNumbers.Select(i => new InHistoryReturnModel() { ItemNumber = i, InHistory = false })
                                  .ToList();
            else {
                Parallel.ForEach(itemNumbers, item => {
                    returnModel.Add(new InHistoryReturnModel() { InHistory = list.Items.Where(i => i.ItemNumber.Equals(item)).Any(), ItemNumber = item });
                });

                return returnModel.ToList();
            }
        }
        #endregion
    }
}
