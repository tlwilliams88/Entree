using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Messaging
{
	public class NoMessagingServiceRepositoryImpl: IMessagingServiceRepository {
        #region ctor
        public NoMessagingServiceRepositoryImpl() {
        }
        #endregion

        #region methods
        public void CreateMailMessage(MailMessageModel mailMessage) {
            throw new NotImplementedException();
        }

        public int GetUnreadMessagesCount(Guid userId) {
            throw new NotImplementedException();
        }

        public int GetUnreadMessagesCount(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public void MarkAsReadUserMessages(List<UserMessageModel> userMessages) {
            throw new NotImplementedException();
        }

        public void MarkAllReadByUser( UserProfile userId ) {
            throw new NotImplementedException();
        }

        public Core.Models.Configuration.MessageTemplateModel ReadMessageTemplateForKey(string key) {
            throw new NotImplementedException();
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId)
        {
			return new List<UserMessagingPreferenceModel>();
        }

		public Core.Models.Paging.PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, Core.Models.Paging.PagingModel paging)
		{
			throw new NotImplementedException();
		}

        public List<UserMessageModel> ReadUserMessages(UserProfile user) {
            throw new NotImplementedException();
        }

        public bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel)
        {
            throw new NotImplementedException();
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
