﻿using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface ICustomerRepository
    {
        List<Customer> GetCustomers();
        void AddUserToCustomer(Guid customerId, Guid userId, string role);
        void RemoveUserFromCustomer(Guid customerId, Guid userId);
    }
}
