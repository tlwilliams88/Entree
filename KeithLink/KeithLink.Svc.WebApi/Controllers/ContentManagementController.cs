using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.ContentManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Dynamic;

namespace KeithLink.Svc.WebApi.Controllers {
    public class ContentManagementController : BaseController {
        #region attributes
        private readonly IContentManagementLogic _logic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public ContentManagementController(IUserProfileLogic profileLogic, IContentManagementLogic cmsLogic, IEventLogRepository logRepo) : base(profileLogic) {
            _logic = cmsLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
		/// <summary>
        /// Read marketing content from the external CMS
        /// </summary>
        /// <param name="branchId">the branch being viewed</param>
        /// <returns>a number of content items based on settings in the configuration</returns>
        /// <remarks>
        /// jwames - 4/28/2015 - original code
        /// </remarks>
        [HttpGet]
        [ApiKeyedRoute("cms/promoitems/{branchId}")]
        public Models.OperationReturnModel<List<ContentItemViewModel>> ReadMarkeitingContent(string branchId) {
            Models.OperationReturnModel<List<ContentItemViewModel>> retVal = new Models.OperationReturnModel<List<ContentItemViewModel>>();
            try {
                retVal.SuccessResponse = _logic.ReadContentForBranch(branchId);
            } catch (Exception ex) {
                _log.WriteErrorLog("Could not retrieve content from CMS.", ex);
                retVal.ErrorMessage = "Could not load content";
            }

            return retVal;
        }
        #endregion
    } 
}