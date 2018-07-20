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
    [SSOAuthorize]
    /// <summary>
    /// end points for handling user feedback
    /// </summary>
	public class SSOController : BaseController
    {
        #region attributes
        private readonly IUserFeedbackLogic _userFeedbackLogic;
        private readonly IUserProfileLogic _profileLogic;
        private readonly IEventLogRepository _log;
        private readonly INotificationHandler _notificationHandler;
        private readonly IListService _listService;

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
        /// <param name="listService"></param>
        public SSOController(
            IUserProfileLogic profileLogic,
            IListService listService
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
            _listService = listService;
        }

        private void GetSSOUser()
        {
            var headers = ControllerContext.Request.Headers;

            if (headers.Contains("username"))
            {
                var email = headers.GetValues("username").First();

                UserProfileReturn users = _profileLogic.GetUserProfile(email);
                SSOUser = users.UserProfiles[0];

                if (Request.Headers.Contains("userSelectedContext"))
                {
                    this.SelectedUserContext = JsonConvert.DeserializeObject<UserSelectedContext>
                        (Request.Headers.GetValues("userSelectedContext").FirstOrDefault().ToString());
                }


            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Submit user feedback
        /// </summary>
        /// <param name="userFeedback">User Feedback</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("sso/customers")]
        public OperationReturnModel<string> GetCustomers()
        {
            var retVal = new OperationReturnModel<string>() { IsSuccess = false };

            return retVal;
        }

        private Customer GetCustomer(UserSelectedContext userContext, UserProfile user)
        {
            Customer customer =
                _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId)
                ?? user.DefaultCustomer;
            return customer;
        }

        /// <summary>
        /// Retrieve list by type for the SSO user
        /// </summary>
        /// <param name="type">List type</param>
        /// <param name="headerOnly">Header only or details?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("sso/list/type")]
        public OperationReturnModel<List<ListModelShallowPrices>> List([FromUri] ListType type = ListType.Worksheet, [FromUri] bool headerOnly = false)
        {
            OperationReturnModel<List<ListModelShallowPrices>> ret = new OperationReturnModel<List<ListModelShallowPrices>>();

            GetSSOUser();

            try
            {
                var lists = _listService.ReadListByType(SSOUser, this.SelectedUserContext, type, headerOnly);
                ret.SuccessResponse = new List<ListModelShallowPrices>();
                foreach(var list in lists)
                {
                    ret.SuccessResponse.Add(list.ToShallowPricesModel());
                }
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                //_elRepo.WriteErrorLog("List", ex);
            }
            return ret;
        }

        #endregion
    }
}