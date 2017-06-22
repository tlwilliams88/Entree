using System;
using System.Collections.Generic;
using System.Web.Http;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.WebApi.Models;

namespace KeithLink.Svc.WebApi.Controllers {
    /// <summary>
    ///     recent item controller
    /// </summary>
    public class RecentItemController : BaseController {
        #region ctor
        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="listLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        public RecentItemController(IListLogic listLogic,
                                    IUserProfileLogic profileLogic,
                                    IRecentlyViewedListLogic recentlyViewedLogic,
                                    IRecentlyOrderedListLogic recentlyOrderedLogic,
                                    IListService listService,
                                    IEventLogRepository logRepo) : base(profileLogic) {
            _repo = listLogic;
            _recentlyViewedLogic = recentlyViewedLogic;
            _recentlyOrderedLogic = recentlyOrderedLogic;
            _listService = listService;
            _log = logRepo;
        }
        #endregion

        #region attributes
        private readonly IListLogic _repo;
        private readonly IRecentlyViewedListLogic _recentlyViewedLogic;
        private readonly IRecentlyOrderedListLogic _recentlyOrderedLogic;
        private readonly IListService _listService;
        private readonly IEventLogRepository _log;
        #endregion

        #region methods
        /// <summary>
        ///     Retrieve recently viewed items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("recent/")]
        public OperationReturnModel<List<RecentItem>> Recent() {
            OperationReturnModel<List<RecentItem>> retVal = new OperationReturnModel<List<RecentItem>>();
            try {
                retVal.SuccessResponse = _listService.ReadRecent(AuthenticatedUser, SelectedUserContext);
                retVal.IsSuccess = true;
            } catch (Exception ex) {
                _log.WriteErrorLog("Recent", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Get the recently ordered special order items list
        /// </summary>
        /// <param name="catalog">the catalog identifying a list of recently ordered items</param>
        [HttpGet]
        [ApiKeyedRoute("recent/order/{catalog}")]
        public OperationReturnModel<RecentNonBEKList> RecentOrder(string catalog) {
            OperationReturnModel<RecentNonBEKList> retVal = new OperationReturnModel<RecentNonBEKList>();
            try {
                retVal.SuccessResponse = _listService.ReadRecentOrder(AuthenticatedUser, SelectedUserContext, catalog);
                //retVal.SuccessResponse = _repo.ReadRecentOrder(this.AuthenticatedUser, 
                //    new Core.Models.SiteCatalog.UserSelectedContext() {
                //        CustomerId = SelectedUserContext.CustomerId,
                //        BranchId = catalog });
                retVal.IsSuccess = true;
            } catch (Exception ex) {
                _log.WriteErrorLog("ReadRecentOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Delete recently viewed items
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("recent/")]
        public OperationReturnModel<bool> DeleteRecent() {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
            try {
                _recentlyViewedLogic.DeleteAll(AuthenticatedUser, SelectedUserContext);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            } catch (Exception ex) {
                _log.WriteErrorLog("DeleteRecent SelectedUserContext.CustomerID=" + SelectedUserContext.CustomerId, ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Delete a recently ordered special order items list
        /// </summary>
        /// <param name="catalog">the catalog for the special order items</param>
        [HttpDelete]
        [ApiKeyedRoute("recent/order/{catalog}")]
        public OperationReturnModel<bool> DeleteRecentOrder(string catalog) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
            try {
                _recentlyOrderedLogic.DeleteAll(AuthenticatedUser,
                                                new UserSelectedContext {
                                                                            CustomerId = SelectedUserContext.CustomerId,
                                                                            BranchId = catalog
                                                                        });
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            } catch (Exception ex) {
                _log.WriteErrorLog("DeleteRecent SelectedUserContext.CustomerID=" + SelectedUserContext.CustomerId, ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Add a new recently viewed item
        /// </summary>
        /// <param name="itemnumber"></param>
        [HttpPost]
        [ApiKeyedRoute("recent/{catalogId}/{itemnumber}")]
        public OperationReturnModel<bool> Recent(string catalogId, string itemnumber) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
            try {
                _recentlyViewedLogic.Save(AuthenticatedUser,
                                          SelectedUserContext,
                                          itemnumber,
                                          false /*each*/,
                                          catalogId);

                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            } catch (Exception ex) {
                _log.WriteErrorLog("Recent(string itemnumber)", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Add recently ordered special order items
        /// </summary>
        /// <param name="list">a recentnonbeklist containing the catalog and a list of itemnumbers</param>
        [HttpPost]
        [ApiKeyedRoute("recent/order")]
        public OperationReturnModel<bool> PostRecentOrder(RecentNonBEKList list) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();
            try {
                _recentlyOrderedLogic.PostRecentOrder(AuthenticatedUser,
                                                      new UserSelectedContext {CustomerId = SelectedUserContext.CustomerId, BranchId = list.Catalog}, list);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            } catch (Exception ex) {
                _log.WriteErrorLog("Recent(string itemnumber)", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }
        #endregion
    }
}