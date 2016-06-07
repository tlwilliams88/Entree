using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Interface.Invoices;

using KeithLink.Svc.Core.Models.Invoices; 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Invoices {
    public class TermLogicImpl : ITermLogic {
        #region attributes
        private ITermRepository _repo;
        #endregion

        #region ctor
        public TermLogicImpl(ITermRepository termRepository) {
            _repo = termRepository;
        }
        #endregion

        #region methods
        public TermModel ReadTermInformation(string branchId, string termCode) {
            var intTerm = termCode.ToInt();

            if(!intTerm.HasValue)
                return null;

            var term = _repo.Read(t => t.BranchId.Equals(branchId) && t.TermCode.Equals(intTerm.Value)).FirstOrDefault();

            if(term == null)
                return null;

            return new TermModel() { BranchId = term.BranchId, TermCode = term.TermCode, Description = term.Description, Age1 = term.Age1, Age2 = term.Age2, Age3 = term.Age3, Age4 = term.Age4 };
        }
        #endregion
    }
}
