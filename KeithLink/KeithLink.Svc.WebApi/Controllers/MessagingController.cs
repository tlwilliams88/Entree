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
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="messagingServiceRepository"></param>
		public MessagingController(IUserProfileLogic profileLogic, IMessagingLogic messagingLogic)
			: base(profileLogic) {
            _msgLogic = messagingLogic;
            userProfileLogic = profileLogic;
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
		public PagedResults<UserMessageModel> usermessages(PagingModel paging)
        {
            return _msgLogic.ReadPagedUserMessages(this.AuthenticatedUser, paging);
        }

		/// <summary>
		/// Mark a message as read by the user
		/// </summary>
		[HttpPut]
        [ApiKeyedRoute("messaging/usermessages/markasread")]
        public void UpdateReadMessages()
        {
            _msgLogic.MarkAllReadByUser(this.AuthenticatedUser);
        }

		/// <summary>
		/// Retrieve a count for the number of unread messages
		/// </summary>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("messaging/usermessages/unreadcount")]
        public int ReadUnreadMessageCount()
        {
            return _msgLogic.GetUnreadMessagesCount(this.AuthenticatedUser.UserId);
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
            _msgLogic.UpdateMessagingPreferences(messagingPreferenceModel, this.AuthenticatedUser);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
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
            return new Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>>() { SuccessResponse = userProfileLogic.GetMessagingPreferences(this.AuthenticatedUser.UserId) };
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
			return new Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>>() { SuccessResponse = userProfileLogic.GetMessagingPreferencesForCustomer(this.AuthenticatedUser.UserId, customerId, branchId) };
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
            _msgLogic.RegisterPushDevice(this.AuthenticatedUser, pushDeviceModel);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
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
			try
			{
				_msgLogic.CreateMailMessage(mailMessage);
				return new OperationReturnModel<bool>() { SuccessResponse = true };
			}
			catch (Exception ex)
			{
				return new OperationReturnModel<bool>() { ErrorMessage = ex.Message, SuccessResponse = false };
				throw ex;
			}

		}
				
        #endregion
    }
}
