using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Messaging.EF;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IUserMessageRepository : IBaseEFREpository<UserMessage>
    {

    }
}