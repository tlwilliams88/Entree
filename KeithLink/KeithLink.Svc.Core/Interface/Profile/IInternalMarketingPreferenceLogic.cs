using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile
{
	public interface IInternalMarketingPreferenceLogic
	{
		void CreateMarketingPreference(MarketingPreferenceModel preference);
		List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to);
	}
}
