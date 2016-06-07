using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Profile {
    public class MarketingPreferencesLogicImpl : IMarketingPreferencesLogic {
        #region attributes
        private readonly IMarketingPreferencesRepository _prefRepo;
        private readonly IUnitOfWork _uow;
        #endregion

        #region ctor
        public MarketingPreferencesLogicImpl(IUnitOfWork unitOfWork, IMarketingPreferencesRepository marketingPrefRepository) {
            _prefRepo = marketingPrefRepository;
            _uow = unitOfWork;
        }
        #endregion

        #region methods
        public void CreateMarketingPreference(MarketingPreferenceModel preference) {
            var pref = new MarketingPreference() {
                Email = preference.Email,
                BranchId = preference.BranchId,
                CurrentCustomer = preference.CurrentCustomer,
                LearnMore = preference.LearnMore,
                RegisteredOn = DateTime.Now.Date
            };

            _prefRepo.Create(pref);
            _uow.SaveChanges();
        }

        public List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to) {
            var prefs = _prefRepo.Read(m => m.RegisteredOn >= from && m.RegisteredOn <= to);

            return prefs.Select(p => new MarketingPreferenceModel() {
                BranchId = p.BranchId,
                Email = p.Email,
                CurrentCustomer = p.CurrentCustomer,
                LearnMore = p.LearnMore,
                Id = p.Id,
                RegisteredOn = p.RegisteredOn
            }).ToList();
        }
        #endregion
    }
}
