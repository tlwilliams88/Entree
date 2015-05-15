using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.Profile
{
	public class MarketingPreferencesServiceRepositoryImpl: IMarketingPreferencesServiceRepository
	{
		private com.benekeith.ProfileService.IProfileService _client;

		public MarketingPreferencesServiceRepositoryImpl(com.benekeith.ProfileService.IProfileService client)
		{
			_client = client;
		}


		public void CreateMarketingPref(MarketingPreferenceModel preference)
		{
			_client.CreateMarketingPref(preference);
		}

		public List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to)
		{
			return _client.ReadMarketingPreferences(from, to).ToList();
		}
	}
}