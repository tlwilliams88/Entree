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
        IDivisionRepository _divisionRepository;
        IDivisionServiceRepository _divisionServiceRepository;
        #endregion

        public DivisionLogicImpl(IDivisionRepository divisionRepository, IDivisionServiceRepository divisionServiceRepository)
        {
			_divisionRepository = divisionRepository;
			_divisionServiceRepository = divisionServiceRepository;
        }

		public List<Division> GetDivisions()
		{
			var divisions = (_divisionRepository.GetDivisions()).Select(c => c.ToDivision()).ToList();
			var branchsupports = _divisionServiceRepository.ReadAllBranchSupports();
			foreach (var division in divisions)
			{
				division.BranchSupport = branchsupports.Find(c => c.BranchId.Equals(division.Id, StringComparison.InvariantCultureIgnoreCase));
			}

			return divisions;
		}       
    }
}
