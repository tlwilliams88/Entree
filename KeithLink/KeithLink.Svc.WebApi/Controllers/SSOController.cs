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

namespace KeithLink.Svc.WebApi.Controllers
{
    [SSOAuthorize]
    /// <summary>
    /// end points for handling user feedback
    /// </summary>
	public class SSOController : BaseController
    {
        #region attributes
        private readonly IUserProfileLogic _profileLogic;

        /// <summary>
        /// SSOUser in SSOController
        /// </summary>
        public UserProfile SSOUser { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        public SSOController(
            IUserProfileLogic profileLogic
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
        }
        #endregion

        #region methods

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
        [ApiKeyedRoute("sso/customer/")]
        public OperationReturnModel<PagedResults<Customer>> SearchCustomers([FromUri] PagingModel paging, [FromUri] SortInfo sort, [FromUri] string account = "",
                                                                                    [FromUri] string terms = "", [FromUri] string type = "1")
        {
            OperationReturnModel<PagedResults<Customer>> retVal = new OperationReturnModel<PagedResults<Customer>>();

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

                var headers = this.ControllerContext.Request.Headers;

                if (headers.Contains("username"))
                {
                    var email = headers.GetValues("username").First();

                    UserProfileReturn users = _profileLogic.GetUserProfile(email);
                    SSOUser = users.UserProfiles[0];
                }

                retVal.SuccessResponse = _profileLogic.CustomerSearch(SSOUser, terms, paging, account, (CustomerSearchType)typeVal);

                // Set the customers UNFI viewing capabilities
                retVal.SuccessResponse.Results.ForEach(x => x.CanViewUNFI = _profileLogic.CheckCanViewUNFI(this.AuthenticatedUser, x.CustomerNumber, x.CustomerBranch));

                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                //_log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                //_log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        #endregion
    }
}