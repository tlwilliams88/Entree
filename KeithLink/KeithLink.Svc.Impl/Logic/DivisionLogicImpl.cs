using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic
{
    public class DivisionLogicImpl : IDivisionLogic
    {
        #region attributes
		private readonly IBranchSupportRepository _branchSupportRepository;
        private readonly IDivisionRepository _divisionRepository;
        #endregion

        #region ctor
        public DivisionLogicImpl(IDivisionRepository divisionRepository, IBranchSupportRepository branchSupportRepo) {
            _branchSupportRepository = branchSupportRepo;
			_divisionRepository = divisionRepository;
        }
        #endregion

        #region methods
        public List<Division> GetDivisions() {
			var divisions = (_divisionRepository.GetDivisions()).Select(c => c.ToDivision()).ToList();
            var branchsupports = ReadBranchSupport();

            if(branchsupports != null) {
                Dictionary<string, BranchSupportModel> branchDict = branchsupports.ToDictionary(b => b.BranchId.ToLower());

                foreach(var division in divisions) {
                    string branchId = division.Id.ToLower();

                    if(branchDict.ContainsKey(branchId)) {
                        division.BranchSupport = branchDict[branchId];
                    }
                }
            }
			

			return divisions;
		}

        public List<BranchSupportModel> ReadBranchSupport() {
            var branchSupport = _branchSupportRepository.ReadAll();

            if(branchSupport == null) {
                return null;
            } else {
                return branchSupport.Select(b => new BranchSupportModel() { BranchId = b.BranchId,
                                                                            Email = b.Email,
                                                                            SupportPhoneNumber = b.SupportPhoneNumber,
                                                                            TollFreeNumber = b.TollFreeNumber
                                                                          }
                                            )
                                    .ToList();
            }
        }
        #endregion
    }
}
