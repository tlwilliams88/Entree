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

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// end points for handling user feedback
    /// </summary>
	public class UserFeedbackController : BaseController
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
        public UserFeedbackController(
            IUserFeedbackLogic userFeedbackLogic, 
            IUserProfileLogic profileLogic,
            IDivisionLogic divisionLogic,
            Func<NotificationType, INotificationHandler> notificationHandlerFactory,
            IEventLogRepository logRepo
            ) : base(profileLogic)
        {
            _userFeedbackLogic = userFeedbackLogic;
            _profileLogic = profileLogic;
            _divisionLogic = divisionLogic;
            _notificationHandler = notificationHandlerFactory(NotificationType.UserFeedback);
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Submit user feedback
        /// </summary>
        /// <param name="userFeedback">User Feedback</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [ApiKeyedRoute("userfeedback")]
        public OperationReturnModel<string> SubmitUserFeedback(UserFeedback userFeedback)
        {
            var retVal = new OperationReturnModel<string>() { IsSuccess = false };

            try
            {
                (string Name, string EmailAddress) target = GetTarget(SelectedUserContext, AuthenticatedUser, userFeedback.Audience);

                var customer = GetCustomer(SelectedUserContext, AuthenticatedUser);

                var context = new UserFeedbackContext
                {
                    UserId = AuthenticatedUser.UserId,
                    UserFirstName = AuthenticatedUser.FirstName,
                    UserLastName = AuthenticatedUser.LastName,
                    BranchId = customer?.CustomerBranch,
                    CustomerNumber = customer?.CustomerNumber,
                    CustomerName = customer?.CustomerName,
                    SalesRepName = customer?.Dsr?.Name,
                    SourceName = AuthenticatedUser.Name,
                    TargetName = target.Name,
                    SourceEmailAddress = AuthenticatedUser.EmailAddress,
                    TargetEmailAddress = target.EmailAddress,
                };

                var notification = new UserFeedbackNotification()
                {
                    BranchId = customer?.CustomerBranch,
                    CustomerNumber = customer?.CustomerNumber,
                    Audience = userFeedback.Audience,

                    Context = context,
                    UserFeedback = userFeedback,
                };

                _userFeedbackLogic.SaveUserFeedback(context, userFeedback);
                _notificationHandler.ProcessNotification(notification);

                retVal.SuccessResponse = "Feedback processed successfully.";
                retVal.IsSuccess = true;
            }
            catch (ApplicationException axe)
            {
                retVal.ErrorMessage = axe.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Application exception", axe);
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = "Could not complete the request. " + ex.Message;
                retVal.IsSuccess = false;
                _log.WriteErrorLog("Unhandled exception", ex);
            }

            return retVal;
        }

        private (string Name, string EmailAddress) GetTarget(UserSelectedContext userContext, UserProfile user, Audience audience)
        {
            (string Name, string EmailAddress) target;

            switch (audience)
            {
                case Audience.Support:
                    target = (audience.ToString(), "entreefeedback@benekeith.com");
                    break;
                case Audience.BranchSupport:
                    var branch = GetBranch(userContext);
                    target = (branch.BranchName, branch.Email);
                    break;
                case Audience.SalesRep:
                    var customer = GetCustomer(userContext, user);
                    target = (customer.Dsr.Name, customer.Dsr.EmailAddress);
                    break;
                case Audience.User:
                    target = (user.Name, user.EmailAddress);
                    break;
                default:
                    target = (audience.ToString(), "entreefeedback@benekeith.com");
                    break;
            }

            // Route feedback to originating user when not in production
            if (Common.Impl.Configuration.IsProduction == false)
            {
                target = ("non-production user", AuthenticatedUser.EmailAddress);
            }

            return target;
        }

        private Customer GetCustomer(UserSelectedContext userContext, UserProfile user)
        {
            Customer customer =
                _profileLogic.GetCustomerForUser(userContext.CustomerId, userContext.BranchId, user.UserId)
                ?? user.DefaultCustomer;
            return customer;
        }

        private BranchSupportModel GetBranch(UserSelectedContext userContext)
        {
            BranchSupportModel branch =
                _divisionLogic.ReadBranchSupport()
                    .Where(theBranch => theBranch.BranchId == userContext.BranchId)
                    .FirstOrDefault();

            return branch;
        }

        #endregion
    }
}