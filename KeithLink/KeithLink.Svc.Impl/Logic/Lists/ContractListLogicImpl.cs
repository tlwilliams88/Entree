using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CloudFront.Model;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class ContractListLogicImpl : IContractListLogic
    {
        #region attributes
        private readonly IContractListDetailsRepository _detailsRepo;
        private readonly IContractListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public ContractListLogicImpl(IContractListHeadersRepository headersRepo, IContractListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            ContractListHeader header = _headersRepo.GetListHeaderForCustomer(catalogInfo);

            if (header != null && headerOnly == false)
            {
               header.Items = _detailsRepo.ReadContractListDetails(header.Id);
            }
            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo)};
            }
            return null;
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(user, catalogInfo, false)[0];
        }
        #endregion
    }
}
