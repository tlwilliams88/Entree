using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Enumerations.Messaging;

namespace Entree.Core.Interface.Messaging
{
    public interface ICustomerTopicRepository : IBaseEFREpository<CustomerTopic>
    {
        List<CustomerTopic> ReadTopicsForCustomer(string customerNumber);
        CustomerTopic ReadTopicForCustomerAndType(string customerNumber, NotificationType notificationType);
    }
}
