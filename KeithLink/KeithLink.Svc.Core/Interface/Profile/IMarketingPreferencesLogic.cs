using KeithLink.Svc.Core.Models.Profile;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IMarketingPreferencesLogic {
        void CreateMarketingPreference(MarketingPreferenceModel preference);

        List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to);
    }
}
