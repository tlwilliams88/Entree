using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface ICustomerTopicRepository : IBaseEFREpository<CustomerTopic>
    {
        List<CustomerTopic> ReadTopicsForCustomer(string customerNumber);
        CustomerTopic ReadTopicForCustomerAndType(string customerNumber, MessageType messageType);
    }
}
