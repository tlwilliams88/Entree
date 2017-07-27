using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Extensions;

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
        public List<string> GetMandatoryItemNumbers(UserProfile profile, UserSelectedContext catalogInfo) {
            List<string> returnValue = new List<string>();
            ListModel list = ReadList(profile, catalogInfo, false);

            if (list != null)
                returnValue.AddRange(list.Items.Select(i => i.ItemNumber)
                                         .ToList());

            return returnValue;
        }

        public ListModel GetListModel(UserProfile profile, UserSelectedContext catalogInfo, long id) {
            return ReadList(profile, catalogInfo, false)
                   .SetUserSpecificProperties(profile);
        }

        public ListModel ReadList(UserProfile profile, UserSelectedContext catalogInfo, bool headerOnly) {
            MandatoryItemsListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);
            List<MandatoryItemsListDetail> items = null;

            if (header != null &&
                headerOnly == false)
                items = _detailsRepo.GetAllByHeader(header.Id);

            return header.ToListModel(items)
                         .SetUserSpecificProperties(profile);
        }

        public void SaveDetail(UserSelectedContext catalogInfo, MandatoryItemsListDetail detail) {
            if (detail.HeaderId == 0) {
                MandatoryItemsListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);

                if (header == null)
                    detail.HeaderId =
                            _headersRepo.SaveMandatoryItemsHeader(new MandatoryItemsListHeader {
                                                                                                   BranchId = catalogInfo.BranchId,
                                                                                                   CustomerNumber = catalogInfo.CustomerId
                                                                                               });
                else
                    detail.HeaderId = header.Id;
            }

            _detailsRepo.Save(detail);
        }

        public void DeleteMandatoryItems(MandatoryItemsListDetail detail) {
            _detailsRepo.Delete(detail.Id);
        }

        public long CreateList(UserProfile user,
                               UserSelectedContext catalogInfo)
        {
            return _headersRepo.SaveMandatoryItemsHeader(new MandatoryItemsListHeader()
            {
                CustomerNumber = catalogInfo.CustomerId,
                BranchId = catalogInfo.BranchId
            });
        }
        #endregion
    }
}