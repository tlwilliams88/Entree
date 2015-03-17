using KeithLink.Svc.Core.Interface.SingleSignOn;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.SingleSignOn {
    public class KbitRequestLogicImpl : IKbitRequestLogic {
        #region attributes
        private IKbitRepository _repo;
        #endregion

        #region ctor
        public KbitRequestLogicImpl(IKbitRepository KbitRepository) {
            _repo = KbitRepository;
        }
        #endregion

        #region methods
        public void UpdateUserAccess(string userName, List<UserSelectedContext> customers) {
            _repo.DeleteAllCustomersForUser(userName);

            foreach (UserSelectedContext customer in customers) {
                _repo.AddCustomerToUser(userName, customer);
            }
        }
        #endregion
    }
}
