using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// end points for handling messaging
    /// </summary>
	[Authorize]
    public class MessagingController : BaseController {
        #region attributes
        private readonly IMessagingLogic _msgLogic;
        private readonly IUserProfileLogic userProfileLogic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="messagingServiceRepository"></param>
		public MessagingController(IUserProfileLogic profileLogic, IMessagingLogic messagingLogic, IEventLogRepository logRepo)
			: base(profileLogic) {
            _msgLogic = messagingLogic;
            userProfileLogic = profileLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve paged list of user messages
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("messaging/usermessages/")]
		public Models.OperationReturnModel<PagedResults<UserMessageModel>> usermessages(PagingModel paging)
        {
            Models.OperationReturnModel<PagedResults<UserMessageModel>> retVal = new Models.OperationReturnModel<PagedResults<UserMessageModel>>();
            try
            {
                retVal.SuccessResponse = _msgLogic.ReadPagedUserMessages(this.AuthenticatedUser, paging);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("usermessages", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

		/// <summary>
		/// Mark a message as read by the user
		/// </summary>
		[HttpPut]
        [ApiKeyedRoute("messaging/usermessages/markasread")]
        public Models.OperationReturnModel<bool> UpdateReadMessages()
        {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _msgLogic.MarkAllReadByUser(this.AuthenticatedUser);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = retVal.SuccessResponse;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("UpdateReadMessages", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

		/// <summary>
		/// Retrieve a count for the number of unread messages
		/// </summary>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("messaging/usermessages/unreadcount")]
        public Models.OperationReturnModel<int> ReadUnreadMessageCount()
        {
            Models.OperationReturnModel<int> retVal = new Models.OperationReturnModel<int>();
            try
            {
                retVal.SuccessResponse = _msgLogic.GetUnreadMessagesCount(this.AuthenticatedUser.UserId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ReadUnreadMessageCount", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

		/// <summary>
		/// Update messages preferences for the authenticated user
		/// </summary>
		/// <param name="messagingPreferenceModel">Updated preferences</param>
		/// <returns></returns>
        [HttpPut]
        [ApiKeyedRoute("messaging/preferences")]
        public Models.OperationReturnModel<bool> UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel)
        {
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            try
            {
                _msgLogic.UpdateMessagingPreferences(messagingPreferenceModel, this.AuthenticatedUser);
                ret.SuccessResponse = true;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("UpdateMessagingPreferences", ex);
                ret.ErrorMessage = ex.Message;
                ret.IsSuccess = false;
            }

            return ret;
        }

		/// <summary>
		/// Retrieve message preferences for the authenticated user
		/// </summary>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("messaging/preferences")]
        public Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>> GetMessagingPreferences()
        {
            Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>> ret = new Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>>();
            try
            {
                ret.SuccessResponse = userProfileLogic.GetMessagingPreferences(this.AuthenticatedUser.UserId);
                ret.IsSuccess = true;
            }
            catch(Exception ex)
            {
                _log.WriteErrorLog("GetMessagingPreferences", ex);
                ret.ErrorMessage = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;
        }

		/// <summary>
		/// Retrieve message preferences for customer
		/// </summary>
		/// <param name="customerId">Customer number</param>
		/// <param name="branchId">Branch id</param>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("messaging/preferences/{customerId}/{branchId}")]
		public Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>> GetMessagingPreferences(string customerId, string branchId)
		{
            Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>> ret = new Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>>();
            try
            {
                ret.SuccessResponse = userProfileLogic.GetMessagingPreferencesForCustomer(this.AuthenticatedUser.UserId, customerId, branchId);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetMessagingPreferences", ex);
                ret.ErrorMessage = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;
        }

        /// <summary>
        /// Register a push device (mobile device)
        /// </summary>
        /// <param name="pushDeviceModel">Registration information</param>
        /// <returns></returns>
        [HttpPut]
        [ApiKeyedRoute("messaging/registerpushdevice")]
        public Models.OperationReturnModel<bool> RegisterPushDeviceToken(PushDeviceRegistrationModel pushDeviceModel)
        {
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            try
            {
                _msgLogic.RegisterPushDevice(this.AuthenticatedUser, pushDeviceModel);
                ret.SuccessResponse = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("RegisterPushDeviceToken", ex);
                ret.ErrorMessage = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;
        }

        /// <summary>
        /// Create a system alert message
        /// </summary>
        /// <param name="mailMessage">Message</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("messaging/createalert")]
        public OperationReturnModel<bool> CreateAlertMessage(MailMessageModel mailMessage)
        {
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try
            {
                mailMessage.IsAlert = true;
                _msgLogic.CreateMailMessage(mailMessage);
                ret.SuccessResponse = true;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.ErrorMessage = ex.Message;
                ret.SuccessResponse = false;
                ret.IsSuccess = false;
                _log.WriteErrorLog("CreateAlertMessage", ex);
            }
            return ret;
        }

        /// <summary>
        /// Create a mail message
        /// </summary>
        /// <param name="mailMessage">Message</param>
        /// <returns></returns>
        [HttpPost]
		[ApiKeyedRoute("messaging/mail")]
		public OperationReturnModel<bool> CreateMessage(MailMessageModel mailMessage)
		{
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try
            {
                _msgLogic.CreateMailMessage(mailMessage);
                ret.SuccessResponse = true;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("CreateMessage", ex);
                ret.ErrorMessage = ex.Message;
                ret.SuccessResponse = false;
                ret.IsSuccess = false;
            }
            return ret;
        }

        /// <summary>
        /// Send a message to the DSR to request contact about a certain item
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("messaging/RequestDSRContact")]
        public OperationReturnModel<int>  RequestDSRContact(string itemnumber)
        {
            OperationReturnModel<int> ret = new OperationReturnModel<int>();
            try
            {
                ret.SuccessResponse = _msgLogic.RequestDSRContact(this.AuthenticatedUser, this.SelectedUserContext, itemnumber);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("RequestDSRContact", ex);
                ret.ErrorMessage = ex.Message;
                ret.SuccessResponse = -1;
                ret.IsSuccess = false;
            }
            return ret;
        }
        #endregion
    }
}
