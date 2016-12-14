using KeithLink.Svc.Core.Extensions.CustomInventoryItems;

using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;

using KeithLink.Svc.WebApi.Helpers;
using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// CustomInventoryController
    /// </summary>
    [Authorize]
    public class CustomInventoryController : BaseController
    {
        #region attributes
        private ICustomInventoryItemsRepository _customInventoryRepo;
        private ICacheRepository _cache;
        private IEventLogRepository _logger;

        private const string CACHE_GROUPNAME = "UserList";
        private const string CACHE_NAME = "UserList";
        private const string CACHE_PREFIX = "Default";
        #endregion

        #region constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="customInventoryRepo"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public CustomInventoryController(IUserProfileLogic profileLogic, ICustomInventoryItemsRepository customInventoryRepo, IEventLogRepository logger, ICacheRepository cache) : base(profileLogic) {
            _customInventoryRepo = customInventoryRepo;
            _cache = cache;
            _logger = logger;
        }
        #endregion

        #region GET

        /// <summary>
        /// Get all custom inventory items by branch and customer number
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("custominventory")]
        public OperationReturnModel<CustomInventoryHeaderReturnModel> GetAll() {
            OperationReturnModel<CustomInventoryHeaderReturnModel> results = new OperationReturnModel<CustomInventoryHeaderReturnModel>();

            try {
                results.SuccessResponse = new CustomInventoryHeaderReturnModel();
                results.SuccessResponse.Items = _customInventoryRepo.GetItemsByBranchAndCustomer(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId).ToReturnModelList();
                results.IsSuccess = true;
            }
            catch (Exception ex) {
                results.IsSuccess = false;
                results.ErrorMessage = ex.Message;

                _logger.WriteErrorLog(String.Format("Error getting custom inventory items: {0}", ex.Message), ex);
            }

            return results;
        } 
        #endregion

        #region POST
        /// <summary>
        /// Save custom inventory items
        /// </summary>
        [HttpPost]
        [ApiKeyedRoute("custominventory")]
        public OperationReturnModel<CustomInventoryHeaderReturnModel> Save(List<CustomInventoryItemReturnModel> items) {
            OperationReturnModel<CustomInventoryHeaderReturnModel> returnValue = new OperationReturnModel<CustomInventoryHeaderReturnModel>();
            returnValue.SuccessResponse = new CustomInventoryHeaderReturnModel();

            // We set the branch and customer number to the proper context because the front end
            // Does not have any concept of this link. 
            foreach(CustomInventoryItemReturnModel item in items) {
                SetCustomerNumberAndBranch(item);
            }

            try {
                _customInventoryRepo.SaveRange(items.ToModel());
                returnValue.IsSuccess = true;
                returnValue.SuccessResponse.Items = _customInventoryRepo.GetItemsByBranchAndCustomer(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId).ToReturnModelList();
            } catch (Exception ex) {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;

                _logger.WriteErrorLog("Error saving custom inventory items", ex);
            }

            return returnValue;
        }
        #endregion

        #region PUT
        #endregion

        #region DELETE
        /// <summary>
        /// Delete a single instance of an item
        /// </summary>
        /// <param name="itemId"></param>
        [HttpDelete]
        [ApiKeyedRoute("custominventory/{itemId}")]
        public void Delete(long itemId) {
            try {
                _customInventoryRepo.Delete(itemId);
            } catch (Exception ex) {
                _logger.WriteErrorLog("Error deleting custom inventory item: ", ex);
            }
        }

        /// <summary>
        /// Delete a list of CustomInventoryItems
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("custominventory/delete")]
        public OperationReturnModel<CustomInventoryHeaderReturnModel> DeleteRange(List<CustomInventoryItemReturnModel> items) {
            OperationReturnModel<CustomInventoryHeaderReturnModel> returnValue = new OperationReturnModel<CustomInventoryHeaderReturnModel>();
            returnValue.SuccessResponse = new CustomInventoryHeaderReturnModel();

            try {
                _customInventoryRepo.DeleteRange(items.ToModel());
                returnValue.IsSuccess = true;
                returnValue.SuccessResponse.Items = _customInventoryRepo.GetItemsByBranchAndCustomer(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId).ToReturnModelList();
            } catch (Exception ex) {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _logger.WriteErrorLog("Error deleting custom inventory items:", ex);
            }

            return returnValue;
        }
        #endregion

        #region helpers
        private void SetCustomerNumberAndBranch(CustomInventoryItemReturnModel item) {
            item.CustomerNumber = this.SelectedUserContext.CustomerId;
            item.BranchId = this.SelectedUserContext.BranchId;
        }
        #endregion
    }
}
