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
        private readonly IDivisionLogic _divisionLogic;
        private readonly IEventLogRepository _log;
        private readonly INotificationHandler _notificationHandler;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="userFeedbackLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="divisionLogic"></param>
        /// <param name="notificationHandlerFactory"></param>
        /// <param name="logRepo"></param>
        public SSOController(
            IUserProfileLogic profileLogic
            ) : base(profileLogic)
        {
            _profileLogic = profileLogic;
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

        public OperationReturnModel<string> Fail()
        {
            var retVal = new OperationReturnModel<string>() { IsSuccess = false };

            retVal.ErrorMessage = "Invalid Credentials";

            return retVal;
        }

        private Customer GetCustomer(UserSelectedContext userContext, UserProfile user)
        {
            Customer customer =
                _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId)
                ?? user.DefaultCustomer;
            return customer;
        }

        #endregion
    }
}