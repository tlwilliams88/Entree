using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    public interface ICustomerContainerRepository
    {
        public void CreateCustomerContainer(string customerName);
        public void DeleteCustomerContainer(string customerName);
        public CustomerContainerReturn GetCustomerContainer(string customerName);
        public CustomerContainerReturn SearchCustomerContainers(string searchText);
    }
}
