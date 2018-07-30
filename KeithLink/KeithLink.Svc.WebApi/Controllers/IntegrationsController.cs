using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.UserFeedback;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.WebApi.Attribute;
using KeithLink.Svc.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// Endpoints for application integrations
    /// </summary>
    [SSOAuthorize]
	public class IntegrationsController : BaseIntegrationsController
    {
        #region attributes
        private readonly IUserProfileLogic _profileLogic;
        private readonly IListService _listService;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="listService"></param>
        public IntegrationsController(
            IUserProfileLogic profileLogic,
            IListService listService
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
            _listService = listService;
        }
        #endregion

        #region methods
        /// <summary>
        /// Get customers user has access to
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("sso/customers")]
        public OperationReturnModel<string> GetCustomers()
        {
            var retVal = new OperationReturnModel<string>() { IsSuccess = false };

            return retVal;
        }

        /// <summary>
        /// Retrieve all list for the SSO user
        /// </summary>
        /// <param name="headeronly">Headonly only or details?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("integrations/lists")]
        public OperationReturnModel<List<ListModelIntegrationsReturnModel>> List(bool headeronly = true)
        {
            OperationReturnModel<List<ListModelIntegrationsReturnModel>> ret = new OperationReturnModel<List<ListModelIntegrationsReturnModel>>();

            try
            {
                var lists = _listService.ReadUserList(SsoUser, this.SelectedUserContext, headeronly);
                ret.SuccessResponse = new List<ListModelIntegrationsReturnModel>();
                foreach (var list in lists)
                {
                    ret.SuccessResponse.Add(list.ToListModelIntegrationsReturnModel());
                }
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// Retrieve a specific list
        /// </summary>
        /// <param name="listId">Lsit id</param>
        /// <param name="includePrice">Include item prices?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("integrations/list/{type}/{listId}")]
        public OperationReturnModel<ListModelIntegrationsReturnModel> List(ListType type, long listId, bool includePrice = true)
        {
            OperationReturnModel<ListModelIntegrationsReturnModel> ret = new OperationReturnModel<ListModelIntegrationsReturnModel>();

            try
            {
                var list = _listService.ReadList(SsoUser, this.SelectedUserContext, type, listId, includePrice);

                ret.SuccessResponse = list.ToListModelIntegrationsReturnModel();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }
        #endregion
    }
}