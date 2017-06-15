using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class MandatoryItemsListLogicImpl : IMandatoryItemsListLogic {
        #region ctor
        public MandatoryItemsListLogicImpl(IMandatoryItemsListHeadersRepository headersRepo, IMandatoryItemsListDetailsRepository detailsRepo) {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region attributes
        private readonly IMandatoryItemsListDetailsRepository _detailsRepo;
        private readonly IMandatoryItemsListHeadersRepository _headersRepo;
        #endregion

        #region methods
        public List<string> GetMandatoryItemNumbers(UserSelectedContext catalogInfo) {
            List<string> returnValue = new List<string>();
            ListModel list = ReadList(catalogInfo, false);

            if (list != null)
                returnValue.AddRange(list.Items.Select(i => i.ItemNumber)
                                         .ToList());

            return returnValue;
        }

        public ListModel GetListModel(UserProfile profile, UserSelectedContext catalogInfo, long id) {
            return ReadList(catalogInfo, false);
        }

        public ListModel ReadList(UserSelectedContext catalogInfo, bool headerOnly) {
            MandatoryItemsListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);
            List<MandatoryItemsListDetail> items = null;

            if (header != null &&
                headerOnly == false)
                items = _detailsRepo.GetAllByHeader(header.Id);

            return header.ToListModel(items);
        }

        public void SaveDetail(UserSelectedContext catalogInfo, MandatoryItemsListDetail detail) {
            if (detail.ParentMandatoryItemsHeaderId == 0) {
                MandatoryItemsListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);

                if (header == null)
                    detail.ParentMandatoryItemsHeaderId =
                            _headersRepo.SaveMandatoryItemsHeader(new MandatoryItemsListHeader {
                                                                                                   BranchId = catalogInfo.BranchId,
                                                                                                   CustomerNumber = catalogInfo.CustomerId
                                                                                               });
                else
                    detail.ParentMandatoryItemsHeaderId = header.Id;
            }

            _detailsRepo.Save(detail);
        }

        public void DeleteReminderItems(MandatoryItemsListDetail detail) {
            _detailsRepo.Delete(detail.Id);
        }
        #endregion
    }
}