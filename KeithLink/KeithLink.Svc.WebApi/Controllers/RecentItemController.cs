using KeithLink.Common.Core.Interfaces.Logging;
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
    /// <summary>
    /// recent item controller
    /// </summary>
    public class RecentItemController : BaseController {
        #region attributes
        private readonly IListLogic _repo;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        public RecentItemController(IListLogic listLogic,  IUserProfileLogic profileLogic, IEventLogRepository logRepo)  : base(profileLogic) {
            _repo = listLogic;
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
                retVal.SuccessResponse = _repo.ReadRecent(this.AuthenticatedUser, this.SelectedUserContext);
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
        /// Get the recently ordered special order items list
        /// </summary>
        /// <param name="catalog">the catalog identifying a list of recently ordered items</param>
        [HttpGet]
        [ApiKeyedRoute("recent/order/{catalog}")]
        public Models.OperationReturnModel<RecentNonBEKList> RecentOrder(string catalog)
        {
            Models.OperationReturnModel<RecentNonBEKList> retVal = new Models.OperationReturnModel<RecentNonBEKList>();
            try
            {
                retVal.SuccessResponse = _repo.ReadRecentOrder(this.AuthenticatedUser, 
                    new Core.Models.SiteCatalog.UserSelectedContext() {
                        CustomerId = SelectedUserContext.CustomerId,
                        BranchId = catalog });
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ReadRecentOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Delete recently viewed items
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("recent/")]
        public Models.OperationReturnModel<bool> DeleteRecent()
        {            
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _repo.DeleteRecent(this.AuthenticatedUser, this.SelectedUserContext);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DeleteRecent SelectedUserContext.CustomerID=" + SelectedUserContext.CustomerId, ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Delete a recently ordered special order items list
        /// </summary>
        /// <param name="catalog">the catalog for the special order items</param>
        [HttpDelete]
        [ApiKeyedRoute("recent/order/{catalog}")]
        public Models.OperationReturnModel<bool> DeleteRecentOrder(string catalog)
        {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _repo.DeleteRecentlyOrdered(this.AuthenticatedUser, 
                    new Core.Models.SiteCatalog.UserSelectedContext() {
                        CustomerId = SelectedUserContext.CustomerId,
                        BranchId = catalog });
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("DeleteRecent SelectedUserContext.CustomerID=" + SelectedUserContext.CustomerId, ex);
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
                _repo.AddRecentlyViewedItem(this.AuthenticatedUser, this.SelectedUserContext, itemnumber);
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

        /// <summary>
        /// Add recently ordered special order items
        /// </summary>
        /// <param name="list">a recentnonbeklist containing the catalog and a list of itemnumbers</param>
        [HttpPost]
        [ApiKeyedRoute("recent/order")]
        public Models.OperationReturnModel<bool> PostRecentOrder(RecentNonBEKList list)
        {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _repo.AddRecentlyOrderedItems(this.AuthenticatedUser, this.SelectedUserContext, list);
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
