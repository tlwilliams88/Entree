using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core;
using System.Web.Http.Cors;
using System.Dynamic;
using KeithLink.Svc.Core.Models.ContentManagement;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class ContentManagementController : BaseController {
        #region attributes
        IContentManagementServiceRepository contentManagementServiceRepository;
        #endregion

        #region ctor
        public ContentManagementController(IContentManagementServiceRepository contentManagementServiceRepository, IUserProfileLogic profileLogic)
            : base(profileLogic)
        {
            this.contentManagementServiceRepository = contentManagementServiceRepository;
        }
        #endregion

        #region methods
		/// <summary>
		/// Read Content items for branch
		/// </summary>
		/// <param name="branchId">Which branch to read content for</param>
		/// <param name="count">Number of items to return</param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("cms/contentitems/{branchId}/{count}")]
        public Models.OperationReturnModel<List<ContentItemViewModel>> ReadContentItems(string branchId, int count)
        {
            Models.OperationReturnModel<List<ContentItemViewModel>> ret = new Models.OperationReturnModel<List<ContentItemViewModel>>();
            ret.SuccessResponse = this.contentManagementServiceRepository.ReadContentItemsByBranch(branchId, count);
            return ret;
        }

		/// <summary>
		/// Read a specific content item
		/// </summary>
		/// <param name="itemId">Content item Id</param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("cms/contentitem/{itemId}")]
        public Models.OperationReturnModel<ContentItemViewModel> ReadContentItem(int itemId)
        {
            Models.OperationReturnModel<ContentItemViewModel> ret = new Models.OperationReturnModel<ContentItemViewModel>();
            ret.SuccessResponse = this.contentManagementServiceRepository.ReadContentItemById(itemId);
            return ret;
        }

		/// <summary>
		/// Read Promo items for branch
		/// </summary>
		/// <param name="branchId">Which branch to read promos for</param>
		/// <param name="count">Number of items to return</param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("cms/promoitems/{branchId}/{count}")]
        public Models.OperationReturnModel<List<ContentItemViewModel>> ReadPromoItems(string branchId, int count)
        {
            Models.OperationReturnModel<List<ContentItemViewModel>> ret = new Models.OperationReturnModel<List<ContentItemViewModel>>();
            ret.SuccessResponse = this.contentManagementServiceRepository.ReadActiveContentItemsByBranch(branchId, count);
            return ret;
        }

		/// <summary>
		/// Create Content item
		/// </summary>
		/// <param name="contentItem">Item to create</param>
		/// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("cms/contentitem")]
        public Models.OperationReturnModel<bool> CreateContentItem(ContentItemPostModel contentItem)
        {
            Models.OperationReturnModel<bool> returnValue = new Models.OperationReturnModel<bool>();

            try {
                this.contentManagementServiceRepository.CreateContentItem( contentItem );
                returnValue.SuccessResponse = true;
            } catch (Exception e) {
                returnValue.SuccessResponse = false;
                returnValue.ErrorMessage = e.Message;
            }

            return returnValue;
        }

		/// <summary>
		/// Delete Content item
		/// </summary>
		/// <param name="itemId">Content item Id to delete</param>
		/// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("cms/contentitem/{itemId}")]
        public Models.OperationReturnModel<bool> DeleteContentItem(int itemId)
        {
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            this.contentManagementServiceRepository.DeleteContentItemById(itemId);
            ret.SuccessResponse = true;
            return ret;
        }

        #endregion
    } 
}