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
        [HttpGet]
        [ApiKeyedRoute("usermessages/")]
        public List<UserMessageModel> usermessages()
        {
            return messagingServiceRepository.ReadUserMessages(this.AuthenticatedUser);
        }

        [HttpPut]
        [ApiKeyedRoute("usermessages/markasread")]
        public void Put(List<UserMessageModel> updatedUserMessages)
        {
            messagingServiceRepository.MarkAsReadUserMessages(updatedUserMessages);
        }

        [HttpGet]
        [ApiKeyedRoute("usermessages/unreadcount")]
        public int unreadmessagescount()
        {
            return messagingServiceRepository.GetUnreadMessagesCount(this.AuthenticatedUser);
        }

        [HttpPut]
        [ApiKeyedRoute("messagingpreferences/")]
        public Models.OperationReturnModel<bool> Put(ProfileMessagingPreferenceModel messagingPreferenceModel)
        {
            messagingServiceRepository.UpdateMessagingPreferences(messagingPreferenceModel, this.AuthenticatedUser);
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
            return ret;
        }

		
        #endregion
    }
}
