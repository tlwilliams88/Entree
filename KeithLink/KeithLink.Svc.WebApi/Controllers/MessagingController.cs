using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class MessagingController : BaseController {
        #region attributes
        private readonly IMessagingServiceRepository messagingServiceRepository;
        #endregion

        #region ctor
		public MessagingController(IUserProfileLogic profileLogic, IMessagingServiceRepository messagingServiceRepository)
			: base(profileLogic)
		{
            this.messagingServiceRepository = messagingServiceRepository;
        }
        #endregion

        #region methods
        [HttpPost]
        [ApiKeyedRoute("messaging/usermessages/")]
		public PagedResults<UserMessageModel> usermessages(PagingModel paging)
        {
            return messagingServiceRepository.ReadPagedUserMessages(this.AuthenticatedUser, paging);
        }

		[HttpGet]
		[ApiKeyedRoute("pagingtest/")]
		public PagingModel Test()
		{
			return new PagingModel() { From = 5, Size = 5, Filter = new FilterInfo() { Field = "subject", Value = "test", Filters = new List<FilterInfo>() { new FilterInfo() { Field = "body", Value = "test" } } } };
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
            return messagingServiceRepository.GetUnreadMessagesCount(this.AuthenticatedUser);
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

        [HttpPut]
        [ApiKeyedRoute("messaging/registerpushdevice")]
        public Models.OperationReturnModel<bool> RegisterPushDeviceToken(PushDeviceRegistrationModel pushDeviceModel)
        {
            messagingServiceRepository.RegisterPushDevice(this.AuthenticatedUser, pushDeviceModel);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
            return ret;
        }		
        #endregion
    }
}
