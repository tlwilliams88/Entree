using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile
{
	public class MarketingPreferencesRepositoryImpl : EFBaseRepository<MarketingPreference>, IMarketingPreferencesRepository
	{
		public MarketingPreferencesRepositoryImpl(IUnitOfWork unitOfWork)
			: base(unitOfWork)
		{            
        }
	}
}
