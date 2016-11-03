using KeithLink.Svc.Core.Extensions.CustomInventoryItems;

using KeithLink.Common.Core.Interfaces.Logging;

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
    [Authorize]
    public class CustomInventoryController : BaseController
    {
        #region attributes
        private ICustomInventoryItemsRepository _customInventoryRepo;
        private IEventLogRepository _logger;
        #endregion

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="customInventoryRepo"></param>
        public CustomInventoryController(IUserProfileLogic profileLogic, ICustomInventoryItemsRepository customInventoryRepo, IEventLogRepository logger) : base(profileLogic) {
            _customInventoryRepo = customInventoryRepo;
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
        public OperationReturnModel<List<CustomInventoryItemReturnModel>> GetAll() {
            OperationReturnModel<List<CustomInventoryItemReturnModel>> results = new OperationReturnModel<List<CustomInventoryItemReturnModel>>();

            try {
                results.SuccessResponse = _customInventoryRepo.GetItemsByBranchAndCustomer(this.SelectedUserContext.BranchId, this.SelectedUserContext.CustomerId).ToReturnModelList();
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
        public void Save(List<CustomInventoryItemReturnModel> items) {
            // We set the branch and customer number to the proper context because the front end
            // Does not have any concept of this link. 
            foreach(CustomInventoryItemReturnModel item in items) {
                SetCustomerNumberAndBranch(item);
            }

            try {
                _customInventoryRepo.SaveRange(items.ToModel());
            } catch (Exception ex) {
                _logger.WriteErrorLog("Error saving custom inventory items", ex);
            }
        }
        #endregion

        #region PUT
        #endregion

        #region DELETE
        /// <summary>
        /// Delete a single instance of an item
        /// </summary>
        /// <param name="itemId"></param>
        [HttpPost]
        [ApiKeyedRoute("custominventory")]
        public void Delete(long itemId) {
            _customInventoryRepo.Delete(itemId);
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
