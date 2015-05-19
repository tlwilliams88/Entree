using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Profile.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalMarketingPreferenceLogicImpl: IInternalMarketingPreferenceLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IMarketingPreferencesRepository marketingPrefRepository;

		public InternalMarketingPreferenceLogicImpl(IUnitOfWork unitOfWork, IMarketingPreferencesRepository marketingPrefRepository)
		{
			this.unitOfWork = unitOfWork;
			this.marketingPrefRepository = marketingPrefRepository;
		}

		public void CreateMarketingPreference(MarketingPreferenceModel preference)
		{
			var pref = new MarketingPreference()
			{
				Email = preference.Email,
				BranchId = preference.BranchId,
				CurrentCustomer = preference.CurrentCustomer,
				LearnMore = preference.LearnMore,
				RegisteredOn = DateTime.Now.Date
			};

			marketingPrefRepository.Create(pref);
			unitOfWork.SaveChanges();
		}

		public List<MarketingPreferenceModel> ReadMarketingPreferences(DateTime from, DateTime to)
		{
			var prefs = marketingPrefRepository.Read(m => m.RegisteredOn >= from && m.RegisteredOn <= to);

			return prefs.Select(p => new MarketingPreferenceModel() { 
				BranchId = p.BranchId,
				Email = p.Email,
				CurrentCustomer = p.CurrentCustomer,
				LearnMore = p.LearnMore,
				Id = p.Id,
				RegisteredOn = p.RegisteredOn
			}).ToList();
		}
	}
}
