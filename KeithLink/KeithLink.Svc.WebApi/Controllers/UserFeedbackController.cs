using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.UserFeedback;
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
    /// end points for working the user profile
    /// </summary>
	public class UserFeedbackController : BaseController
    {
        #region attributes
        private readonly IUserFeedbackLogic _userFeedbackLogic;
        private readonly IUserProfileLogic _profileLogic;
        private readonly IDivisionLogic _divisionLogic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="userFeedbackLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="divisionLogic"></param>
        /// <param name="logRepo"></param>
        public UserFeedbackController(
            IUserFeedbackLogic userFeedbackLogic, 
            IUserProfileLogic profileLogic,
            IDivisionLogic divisionLogic,
            //IMessagingLogic messagingLogic,
            //IMessageProvider emailMessageProvider,
            //IMessageTemplateLogic messageTemplateLogic,
            IEventLogRepository logRepo
            ) : base(profileLogic)
        {
            _userFeedbackLogic = userFeedbackLogic;
            _profileLogic = profileLogic;
            _divisionLogic = divisionLogic;
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
        public OperationReturnModel<int> SubmitUserFeedback(UserFeedback userFeedback)
        {
            OperationReturnModel<int> retVal = new OperationReturnModel<int>() { IsSuccess = false };

            try
            {
                var target = GetTarget(SelectedUserContext, AuthenticatedUser, userFeedback.Audience);

                var customer = GetCustomer(SelectedUserContext, AuthenticatedUser);

                var context = new UserFeedbackContext
                {
                    UserId = AuthenticatedUser.UserId,
                    BranchId = customer?.CustomerBranch,
                    CustomerName = customer?.CustomerName,
                    SalesRepName = customer?.Dsr?.Name,
                    SourceName = AuthenticatedUser.Name,
                    TargetName = target.Item1,
                    SourceEmailAddress = AuthenticatedUser.EmailAddress,
                    TargetEmailAddress = target.Item2,
                };

                retVal.SuccessResponse = _userFeedbackLogic.SubmitUserFeedback(context, userFeedback);
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

        private Tuple<string, string> GetTarget(UserSelectedContext userContext, UserProfile user, Audience audience)
        {
            Tuple<string, string> target = null;

            switch (audience)
            {
                case Audience.ProductSupport:
                    target = Tuple.Create(audience.ToString(), "entreefeedback@benekeith.com");
                    break;
                case Audience.BranchSupport:
                    var branch = GetBranch(userContext);
                    target = Tuple.Create(branch.BranchName, branch.Email);
                    break;
                case Audience.SalesRep:
                    var customer = GetCustomer(userContext, user);
                    target = Tuple.Create(customer.Dsr.Name, customer.Dsr.EmailAddress);
                    break;
                case Audience.User:
                    target = Tuple.Create(user.Name, user.EmailAddress);
                    break;
                default:
                    break;
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