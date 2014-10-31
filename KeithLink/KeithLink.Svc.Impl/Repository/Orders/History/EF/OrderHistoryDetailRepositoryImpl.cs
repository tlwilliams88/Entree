using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders.History.EF {
    public class OrderHistoryDetailRepositoryImpl : EFBaseRepository<OrderHistoryDetail>, IOrderHistoryDetailRepository {
        #region ctor
        public OrderHistoryDetailRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
    }
}
