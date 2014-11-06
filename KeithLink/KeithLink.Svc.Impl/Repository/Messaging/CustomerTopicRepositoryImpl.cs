using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Repository.Messaging
{
    public class CustomerTopicRepositoryImpl : EFBaseRepository<CustomerTopic>, ICustomerTopicRepository
    {
        public CustomerTopicRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<CustomerTopic> ReadTopicsForCustomer(string customerNumber)
        {
            throw new NotImplementedException();
        }

        public CustomerTopic ReadTopicForCustomerAndType(string customerNumber, NotificationType notificationType)
        {
            return Entities.Where(t => t.NotificationType == notificationType && t.CustomerNumber == customerNumber).FirstOrDefault();
        }
    }
}
