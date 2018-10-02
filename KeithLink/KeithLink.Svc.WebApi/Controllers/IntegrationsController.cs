using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.UserFeedback;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.UserFeedback;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Linq;
using System.Web.Http;
using KeithLink.Svc.WebApi.Attribute;
using KeithLink.Svc.Core.Models.Paging;
using System.Collections.Generic;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Customers;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using Newtonsoft.Json;

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
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="listService"></param>
        public IntegrationsController(
            IUserProfileLogic profileLogic,
            IListService listService,
            IEventLogRepository log
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
            _listService = listService;
            _log = log;
        }
        #endregion

        #region methods

        /// <summary>
        /// Paged search of customers
        /// </summary>
        /// <param name="paging">Paging information</param>
        /// <param name="sort">Sort object</param>
        /// <param name="terms">Search text</param>
        /// <param name="type">The type of text we are searching for. Is converted to CustomerSearchType enumerator</param>
        /// <returns>search results as a paged list of customers</returns>
        [HttpGet]
        [ApiKeyedRoute("integrations/customer/")]
        public OperationReturnModel<PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel>> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort,
                                                                                    [FromUri] string terms = "", [FromUri] string type = "1")
        {
            OperationReturnModel<PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel>> retVal = new OperationReturnModel<PagedResultsForCustomersIntegration<CustomerIntegrationsReturnModel>>();
            string account = "";

            try
            {
                if (paging.Sort == null && sort != null && !String.IsNullOrEmpty(sort.Order) && !String.IsNullOrEmpty(sort.Field))
                {
                    paging.Sort = new List<SortInfo>() { sort };
                }

                int typeVal;
                if (!int.TryParse(type, out typeVal))
                {
                    typeVal = 1;
                }

                PagedResults<Customer> customers =
                    _profileLogic.CustomerSearch(SsoUser, terms, paging, account, (CustomerSearchType) typeVal);
                retVal.SuccessResponse = customers.ToPagedForCustomersIntegration();

                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                _log.WriteErrorLog("CustomerSearch", axe);
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("CustomerSearch-ex", ex);
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
            }

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
                _log.WriteErrorLog("ReadUserList", ex);
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
                _log.WriteErrorLog("ReadList", ex);
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }
        #endregion
    }
}