using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.WebApi.Models;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class MessagingController : BaseController {
        #region attributes
        private readonly IMessagingServiceRepository messagingServiceRepository;
        private readonly IUserProfileLogic userProfileLogic;
        #endregion

        #region ctor
		public MessagingController(IUserProfileLogic profileLogic, IMessagingServiceRepository messagingServiceRepository)
			: base(profileLogic)
		{
            this.messagingServiceRepository = messagingServiceRepository;
            this.userProfileLogic = profileLogic;
        }
        #endregion

        #region methods
        [HttpPost]
        [ApiKeyedRoute("messaging/usermessages/")]
		public PagedResults<UserMessageModel> usermessages(PagingModel paging)
        {
            return messagingServiceRepository.ReadPagedUserMessages(this.AuthenticatedUser, paging);
        }

		[HttpPut]
        [ApiKeyedRoute("messaging/usermessages/markasread")]
        public void UpdateReadMessages(List<UserMessageModel> updatedUserMessages)
        {
            messagingServiceRepository.MarkAsReadUserMessages(updatedUserMessages);
        }

        [HttpGet]
        [ApiKeyedRoute("messaging/usermessages/unreadcount")]
        public int ReadUnreadMessageCount()
        {
            return messagingServiceRepository.GetUnreadMessagesCount(this.AuthenticatedUser.UserId);
        }

        [HttpPut]
        [ApiKeyedRoute("messaging/preferences")]
        public Models.OperationReturnModel<bool> UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel)
        {
            messagingServiceRepository.UpdateMessagingPreferences(messagingPreferenceModel, this.AuthenticatedUser);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
            return ret;
        }

        [HttpGet]
        [ApiKeyedRoute("messaging/preferences")]
        public Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>> GetMessagingPreferences()
        {
            return new Models.OperationReturnModel<List<ProfileMessagingPreferenceModel>>() { SuccessResponse = userProfileLogic.GetMessagingPreferences(this.AuthenticatedUser.UserId) };
        }

        [HttpPut]
        [ApiKeyedRoute("messaging/registerpushdevice")]
        public Models.OperationReturnModel<bool> RegisterPushDeviceToken(PushDeviceRegistrationModel pushDeviceModel)
        {
            messagingServiceRepository.RegisterPushDevice(this.AuthenticatedUser, pushDeviceModel);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
            return ret;
        }

		[HttpPost]
		[ApiKeyedRoute("messaging/mail")]
		public OperationReturnModel<bool> CreateMessage(MailMessageModel mailMessage)
		{
			try
			{
				messagingServiceRepository.CreateMailMessage(mailMessage);
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
