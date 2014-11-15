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
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class ContentManagementController : BaseController {
        #region attributes
        #endregion

        #region ctor
        public ContentManagementController(IUserProfileLogic profileLogic)
            : base(profileLogic)
        {
            
        }
        #endregion

        #region methods
        [HttpGet]
        [ApiKeyedRoute("cms/contentitems/{branchId}/{count}")]
        public Models.OperationReturnModel<List<ContentItemViewModel>> ReadContentItems(string branchId, int count)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ApiKeyedRoute("cms/contentitem/{branchId}/{itemId}")] // shouldn't need branchId
        public Models.OperationReturnModel<ContentItemViewModel >ReadContentItem(string branchId, string itemId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ApiKeyedRoute("cms/contentitems/promoitems/{branchId}/{count}")]
        public Models.OperationReturnModel<ContentItemViewModel> ReadPromoContentItems(string branchId, int count)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ApiKeyedRoute("cms/contentitem")]
        public Models.OperationReturnModel<bool> CreateContentItem(ContentItemPostModel contentItem)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [ApiKeyedRoute("cms/contentitem/{branchId}/{itemId}")] // shouldn't need branch id
        public Models.OperationReturnModel<bool> DeleteContentItem(string branchId, string itemId)
        {
            throw new NotImplementedException();
        }

        #endregion
    } 
}