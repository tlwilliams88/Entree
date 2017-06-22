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
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// recent item controller
    /// </summary>
    public class RecentItemController : BaseController {
        #region attributes
        private readonly IListLogic _repo;
        private readonly IRecentlyViewedListLogic _recentlyViewedLogic;
        private readonly IRecentlyOrderedListLogic _recentlyOrderedLogic;
        private readonly IListService _listService;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="listLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        public RecentItemController(IListLogic listLogic,  IUserProfileLogic profileLogic, IRecentlyViewedListLogic recentlyViewedLogic,
                                    IRecentlyOrderedListLogic recentlyOrderedLogic, IListService listService, IEventLogRepository logRepo)  : base(profileLogic) {
            _repo = listLogic;
            _recentlyViewedLogic = recentlyViewedLogic;
            _recentlyOrderedLogic = recentlyOrderedLogic;
            _listService = listService;
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
                retVal.SuccessResponse = _listService.ReadRecent(this.AuthenticatedUser, this.SelectedUserContext);
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
                retVal.SuccessResponse = _listService.ReadRecentOrder(this.AuthenticatedUser, this.SelectedUserContext, catalog);
                //retVal.SuccessResponse = _repo.ReadRecentOrder(this.AuthenticatedUser, 
                //    new Core.Models.SiteCatalog.UserSelectedContext() {
                //        CustomerId = SelectedUserContext.CustomerId,
                //        BranchId = catalog });
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
                //_recentlyViewedLogic.DeleteRecentlyViewed(this.AuthenticatedUser, this.SelectedUserContext);
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
                _recentlyOrderedLogic.DeleteAll(this.AuthenticatedUser,
                    new Core.Models.SiteCatalog.UserSelectedContext()
                    {
                        CustomerId = SelectedUserContext.CustomerId,
                        BranchId = catalog
                    });
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
                //_recentlyViewedLogic.AddOrUpdateRecentlyViewed(this.AuthenticatedUser, 
                //                                               this.SelectedUserContext,
                //                                               itemnumber,
                //                                               false/*each*/,
                //                                               this.SelectedUserContext.BranchId,
                //                                               true/*active*/);
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
                _recentlyOrderedLogic.PostRecentOrder(this.AuthenticatedUser, 
                    new UserSelectedContext() { CustomerId = SelectedUserContext.CustomerId, BranchId = list.Catalog }, list);
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
