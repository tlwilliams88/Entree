using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic {
    public class DivisionLogicImpl : IDivisionLogic {
        #region ctor
        public DivisionLogicImpl(IDivisionRepository divisionRepository, IBranchSupportRepository branchSupportRepo) {
            _branchSupportRepository = branchSupportRepo;
            _divisionRepository = divisionRepository;
        }
        #endregion

        #region attributes
        private readonly IBranchSupportRepository _branchSupportRepository;
        private readonly IDivisionRepository _divisionRepository;
        #endregion

        #region methods
        public List<Division> GetDivisions() {
            List<Division> divisions = _divisionRepository.GetDivisions()
                                                          .Select(c => c.ToDivision())
                                                          .ToList();
            List<BranchSupportModel> branchsupports = ReadBranchSupport();

            if (branchsupports != null) {
                Dictionary<string, BranchSupportModel> branchDict = branchsupports.ToDictionary(b => b.BranchId.ToLower());

                foreach (Division division in divisions) {
                    string branchId = division.Id.ToLower();

                    if (branchDict.ContainsKey(branchId)) {
                        division.BranchSupport = branchDict[branchId];
                    }
                }
            }

            return divisions;
        }

        public List<BranchSupportModel> ReadBranchSupport() {
            IEnumerable<BranchSupport> branchSupport = _branchSupportRepository.ReadAll();

            if (branchSupport == null)
                return null;
            return branchSupport.Select(b => new BranchSupportModel {
                                            BranchId = b.BranchId,
                                            BranchName = b.BranchName,
                                            Email = b.Email,
                                            SupportPhoneNumber = b.SupportPhoneNumber,
                                            TollFreeNumber = b.TollFreeNumber
                                        }
                                       )
                                .ToList();
        }
        #endregion
    }
}