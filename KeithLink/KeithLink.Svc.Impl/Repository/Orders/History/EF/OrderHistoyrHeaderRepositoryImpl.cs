using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders.History.EF {
    public class OrderHistoyrHeaderRepositoryImpl : EFBaseRepository<OrderHistoryHeader>, IOrderHistoryHeaderRepsitory {
        #region ctor
        public OrderHistoyrHeaderRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
    }
}
