using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.ReminderItem;

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
        public List<string> GetRemindersNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.Select(i => i.ItemNumber).ToList();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            ReminderItemsListHeader header = _headersRepo.GetReminderItemsHeader(user.UserId.ToString(), catalogInfo, headerOnly);

            if (header != null && headerOnly == false)
            {
                header.Items = _detailsRepo.GetRemindersDetails(header.Id);
            }

            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo) };
            }
            return null;
        }

        public void AddOrUpdateReminder(UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateReminder(catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(user, catalogInfo, false)[0];
        }

        public void DeleteReminderItems(UserProfile user, UserSelectedContext catalogInfo)
        {
            _detailsRepo.DeleteReminders(user.UserId.ToString(), catalogInfo.CustomerId, catalogInfo.BranchId);
        }

        #endregion
    }
}
