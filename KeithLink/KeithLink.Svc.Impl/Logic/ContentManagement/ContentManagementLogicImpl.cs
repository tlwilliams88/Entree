using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Extensions.ContentManagement;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Models.ContentManagement;
using EE = KeithLink.Svc.Core.Models.ContentManagement.ExpressEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using Newtonsoft.Json;

namespace KeithLink.Svc.Impl.Logic.ContentManagement {
    public class ContentManagementLogicImpl : IContentManagementLogic {
        #region attributes
        private readonly IEventLogRepository _log;
        private readonly IContentManagementExternalRepository _repo;
        private readonly IAuditLogRepository _audit;
        #endregion

        #region ctor
        public ContentManagementLogicImpl(IEventLogRepository logRepo, IContentManagementExternalRepository contentRepo, IAuditLogRepository audit) {
            _log = logRepo;
            _audit = audit;
            _repo = contentRepo;
        }
        #endregion

        #region methods
        public List<ContentItemViewModel> ReadContentForBranch(string branchId) {
            List<EE.ContentItem> rawItems = _repo.GetAllContent();

            List<ContentItemViewModel> retVal = new List<ContentItemViewModel>();

            retVal.AddRange(GetBranchItems(ref rawItems, branchId));
            retVal.AddRange(GetGlobalItems(ref rawItems, retVal.Count));

            return retVal;
        }

        public bool LogHit(UserProfile user, UserSelectedContext context, ContentItemClickedModel clicked)
        {
            _audit.WriteToAuditLog(
                Common.Core.Enumerations.AuditType.MarketingCampaignClicked, 
                user.EmailAddress,
                string.Format("customer {0}, campaign {1}", JsonConvert.SerializeObject(context), JsonConvert.SerializeObject(clicked))
                );
            return true;
        }

        private List<ContentItemViewModel> GetBranchItems(ref List<EE.ContentItem> items, string branchId)
        {
            return items.Where(i => i.AdditionalData != null
                                    && i.AdditionalData.Count > 0 
                                    && GetBranchName(branchId).Contains(i.AdditionalData[0].TargetBranch, StringComparer.CurrentCultureIgnoreCase))
                        .Select(i => i.ToContentItemViewModel(branchId))
                        .ToList();
        }

        private List<string> GetBranchName(string branchId)
        {
            List<string> list = new List<string>();
            switch (branchId.ToUpper()) {
                case Constants.BRANCH_FAM:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FAM);
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FAM_ALTERNATE);
                    break;
                case Constants.BRANCH_FAQ:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FAQ);
                    break;
                case Constants.BRANCH_FAR:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FAR);
                    break;
                case Constants.BRANCH_FDF:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FDF);
                    break;
                case Constants.BRANCH_FHS:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FHS);
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FHS_ALTERNATE);
                    break;
                case Constants.BRANCH_FLR:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FLR);
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FLR_ALTERNATE);
                    break;
                case Constants.BRANCH_FOK:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FOK);
                    break;
                case Constants.BRANCH_FEL:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FEL);
                    break;
                case Constants.BRANCH_FSA:
                    list.Add(Constants.CONTENTMGMT_BRANCHNAME_FSA);
                    break;
            }
            return list;
        }

        private List<ContentItemViewModel> GetGlobalItems(ref List<EE.ContentItem> items, int existingItemCount) {
            return (from EE.ContentItem i in items
                    where (i.AdditionalData.Count > 0 && 
                            i.AdditionalData[0].TargetBranch.Equals(Constants.CONTENTMGMT_BRANCHNAME_GOF, StringComparison.InvariantCultureIgnoreCase))
                    select i.ToContentItemViewModel(string.Empty))
                    .Take(Configuration.MarketingContentTotalItemCount - existingItemCount)
                    .ToList();
        }
        #endregion
    }
}
