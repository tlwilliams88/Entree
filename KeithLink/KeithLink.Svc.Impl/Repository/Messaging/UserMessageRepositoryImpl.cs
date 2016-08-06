using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Messaging
{
    public class UserMessageRepositoryImpl : EFBaseRepository<UserMessage>, IUserMessageRepository
    {
        public UserMessageRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IEnumerable<UserMessage> ReadUserMessages(Core.Models.Profile.UserProfile user)
        {
            return this.Entities.Where(a => a.UserId == user.UserId);
        }

        public IEnumerable<UserMessage> ReadUserMessagesPaged(Core.Models.Profile.UserProfile user, int? pageSize, int? startFrom) {
            int size = pageSize ?? 50;
            int from = startFrom ?? 0;

            return this.Entities.Where(a => a.UserId == user.UserId).Skip(from).Take(size);
        }


        public IEnumerable<UserMessage> ReadUnreadMessagesByUser( Core.Models.Profile.UserProfile user ) {
            return this.Entities.Where( a => (a.UserId.Equals( user.UserId ) && a.MessageReadUtc.Equals( null )) );
        }

        public int GetUnreadMessagesCount(Guid userId)
        {
            return this.Entities.Count(a => (a.UserId == userId && a.MessageReadUtc.Equals(null)));
        }

    }
}