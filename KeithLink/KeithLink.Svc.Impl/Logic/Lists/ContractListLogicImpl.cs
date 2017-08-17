using System.Collections.Generic;

using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class ContractListLogicImpl : IContractListLogic {
        #region attributes
        private readonly IContractListDetailsRepository _detailsRepo;
        private readonly IContractListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public ContractListLogicImpl(IContractListHeadersRepository headersRepo, IContractListDetailsRepository detailsRepo) {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long id) {
            ListModel list = null;

            ContractListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);

            if (header != null) {
                List<ContractListDetail> items = _detailsRepo.GetContractListDetails(header.Id);

                list = header.ToListModel(items);
            } else {
                list = null;
            }
            return list;
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            ListModel list = null;

            ContractListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);

            if (header != null)
            {
                if (headerOnly) {
                    list = header.ToListModel();
                }
                else {
                    List<ContractListDetail> items = _detailsRepo.GetContractListDetails(header.Id);

                    list = header.ToListModel(items);
                }
            }
            else
            {
                list = null;
            }
            return list;
        }
        #endregion
    }
}
