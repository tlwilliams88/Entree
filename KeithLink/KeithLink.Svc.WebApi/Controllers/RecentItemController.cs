using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class RecentItemController : BaseController {
        #region attributes
        private readonly IListServiceRepository listServiceRepository;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public RecentItemController(IListServiceRepository listServiceRepository,  IUserProfileLogic profileLogic, IEventLogRepository logRepo)  : base(profileLogic) {
			this.listServiceRepository = listServiceRepository;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve recently viewed items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
		[ApiKeyedRoute("recent/")]
		public Models.OperationReturnModel<List<RecentItem>> Recent() {
            Models.OperationReturnModel<List<RecentItem>> retVal = new Models.OperationReturnModel<List<RecentItem>>();
            try
            {
                retVal.SuccessResponse = listServiceRepository.ReadRecent(this.AuthenticatedUser, this.SelectedUserContext);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Recent", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Add a new recently viewed item
        /// </summary>
        /// <param name="itemnumber"></param>
        [HttpPost]
		[ApiKeyedRoute("recent/{itemnumber}")]
		public Models.OperationReturnModel<bool> Recent(string itemnumber) {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                listServiceRepository.AddRecentlyViewedItem(this.AuthenticatedUser, this.SelectedUserContext, itemnumber);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Recent(string itemnumber)", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        #endregion

    }
}
