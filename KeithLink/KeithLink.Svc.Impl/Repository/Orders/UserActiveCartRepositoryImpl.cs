using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders.EF;
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
        
	}
}
