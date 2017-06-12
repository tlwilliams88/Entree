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
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            ContractListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);
            List<ContractListDetail> items = null;

            if (header != null) {
                items = _detailsRepo.GetContractListDetails(header.Id);
            }

            return header.ToListModel(items);
        }
        #endregion
    }
}
