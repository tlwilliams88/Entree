using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface ICustomerContainerRepository
    {
        void CreateCustomerContainer(string customerName);
        void DeleteCustomerContainer(string customerName);
        CustomerContainerReturn GetCustomerContainer(string customerName);
        CustomerContainerReturn SearchCustomerContainers(string searchText);
    }
}
