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
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class MandatoryItemsListLogicImpl : IMandatoryItemsListLogic
    {
        #region attributes
        private readonly IMandatoryItemsListDetailsRepository _detailsRepo;
        private readonly IMandatoryItemsListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public MandatoryItemsListLogicImpl(IMandatoryItemsListHeadersRepository headersRepo, IMandatoryItemsListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public List<string> GetMandatoryItemNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            if (list != null)
            {
                return list[0].Items.Select(i => i.ItemNumber).ToList();
            }
            return new List<string>();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            MandatoryItemsListHeader header = _headersRepo.GetMandatoryItemsHeader(user.UserId.ToString(), catalogInfo, headerOnly);

            if (header != null && headerOnly == false)
            {
                header.Items = _detailsRepo.GetMandatoryItemsDetails(header.Id);
            }

            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo) };
            }
            return null;
        }

        public void AddOrUpdateMandatoryItem(UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateMandatoryItem(catalogInfo.CustomerId,
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
            _detailsRepo.DeleteMandatoryItems(user.UserId.ToString(), catalogInfo.CustomerId, catalogInfo.BranchId);
        }
        #endregion
    }
}
