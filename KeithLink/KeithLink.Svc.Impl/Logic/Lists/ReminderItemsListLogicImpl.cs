using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class ReminderItemsListLogicImpl : IRemindersListLogic
    {
        #region attributes
        private readonly IRemindersListDetailsRepository _detailsRepo;
        private readonly IRemindersListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public ReminderItemsListLogicImpl(IRemindersListHeadersRepository headersRepo, IRemindersListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public List<string> GetRemindersNumbers(UserProfile user, UserSelectedContext catalogInfo) {
            ListModel list = GetListModel(user, catalogInfo, 0);

            return list?.Items.Select(i => i.ItemNumber).ToList();
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

            if (header == null) {
                return null;
            } else {
                List<ReminderItemsListDetail> items = _detailsRepo.GetRemindersDetails(header.Id);

                return header.ToListModel(items);
            }
        }

        public void Save(UserSelectedContext catalogInfo, ReminderItemsListDetail model) {
            // try to find the parent header id if it is not in the model
            if(model.ParentRemindersHeaderId == 0) {
                ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(catalogInfo);

                if(header == null) {
                    // create the header
                    model.ParentRemindersHeaderId = _headersRepo.SaveReminderListHeader(new ReminderItemsListHeader() {
                                                                                            BranchId = catalogInfo.BranchId,
                                                                                            CustomerNumber = catalogInfo.CustomerId
                                                                                        });
                } else {
                    model.ParentRemindersHeaderId = header.Id;
                }
            }

            _detailsRepo.SaveReminderListDetail(model);
        }
        #endregion
    }
}
