﻿using Entree.Core.Interface.Orders;
using Entree.Core.Models.Orders.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class UserActiveCartRepositoryImpl : EFBaseRepository<UserActiveCart>, IUserActiveCartRepository
	{
		public UserActiveCartRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public UserActiveCart GetCurrentUserCart(Guid userId, string branchId, string customerNumber) {
            return Read(u => u.UserId == userId &&
                             u.CustomerId.Equals(customerNumber) &&
                             u.BranchId.Equals(branchId))
                  .FirstOrDefault();
        }
    }
}
