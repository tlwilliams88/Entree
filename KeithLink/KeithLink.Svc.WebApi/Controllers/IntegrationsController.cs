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
using KeithLink.Svc.Core.Enumerations.Profile;
using KeithLink.Svc.Core.Extensions.Customers;
using Newtonsoft.Json;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// Endpoints for application integrations
    /// </summary>
    [SSOAuthorize]
	public class IntegrationsController : BaseController
    {
        #region attributes
        private readonly IUserProfileLogic _profileLogic;

        /// <summary>
        /// SSOUser in SSOController
        /// </summary>
        public UserProfile SsoUser { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        public IntegrationsController(
            IUserProfileLogic profileLogic
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
        }
        #endregion

        #region methods

        private void GetSsoUser()
        {
            var headers = ControllerContext.Request.Headers;

            if (headers.Contains("username"))
            {
                var email = headers.GetValues("username").First();

                UserProfileReturn users = _profileLogic.GetUserProfile(email);
                SsoUser = users.UserProfiles[0];

                if (Request.Headers.Contains("userSelectedContext"))
                {
                    this.SelectedUserContext = JsonConvert.DeserializeObject<UserSelectedContext>
                        (Request.Headers.GetValues("userSelectedContext").FirstOrDefault().ToString());
                }


            }
        }

        /// <summary>
        /// Paged search of customers
        /// </summary>
        /// <param name="paging">Paging information</param>
        /// <param name="sort">Sort object</param>
        /// <param name="account">Account</param>
        /// <param name="terms">Search text</param>
        /// <param name="type">The type of text we are searching for. Is converted to CustomerSearchType enumerator</param>
        /// <returns>search results as a paged list of customers</returns>
        [HttpGet]
        [ApiKeyedRoute("integrations/customer/")]
        public OperationReturnModel<PagedResults<CustomerShallow>> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort,
                                                                                    [FromUri] string terms = "", [FromUri] string type = "1")
        {
            OperationReturnModel<PagedResults<CustomerShallow>> retVal = new OperationReturnModel<PagedResults<CustomerShallow>>();
            string account = "";

            GetSsoUser();

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
                retVal.SuccessResponse = customers.ToPagedCustomerShallow();

                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        #endregion
    }
}