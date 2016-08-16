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
                user.UserName,
                string.Format("customer {0}, campaign:({1}) {2}", context.CustomerId, clicked.CampaignId, clicked.TagLine)
                );
            return true;
        }

        private List<ContentItemViewModel> GetBranchItems(ref List<EE.ContentItem> items, string branchId) {
            return (from EE.ContentItem i in items
                    where i.AdditionalData.Count > 0 &&
                            i.AdditionalData[0].TargetBranch.Contains(GetBranchName(branchId))
                    select i.ToContentItemViewModel(branchId))
                    .Take(Configuration.MarketingContentBranchItemCount)
                    .ToList();
        }

        private string GetBranchName(string branchId) {
            switch (branchId.ToUpper()) {
                case Constants.BRANCH_FAM:
                    return Constants.CONTENTMGMT_BRANCHNAME_FAM;
                case Constants.BRANCH_FAQ:
                    return Constants.CONTENTMGMT_BRANCHNAME_FAQ;
                case Constants.BRANCH_FAR:
                    return Constants.CONTENTMGMT_BRANCHNAME_FAR;
                case Constants.BRANCH_FDF:
                    return Constants.CONTENTMGMT_BRANCHNAME_FDF;
                case Constants.BRANCH_FHS:
                    return Constants.CONTENTMGMT_BRANCHNAME_FHS;
                case Constants.BRANCH_FLR:
                    return Constants.CONTENTMGMT_BRANCHNAME_FLR;
                case Constants.BRANCH_FOK:
                    return Constants.CONTENTMGMT_BRANCHNAME_FOK;
                case Constants.BRANCH_FSA:
                    return Constants.CONTENTMGMT_BRANCHNAME_FSA;
                default:
                    return null;
            }
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
