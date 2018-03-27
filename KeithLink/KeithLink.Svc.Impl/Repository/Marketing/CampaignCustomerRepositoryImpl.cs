using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Impl.Repository.DataConnection;

namespace KeithLink.Svc.Impl.Repository.Marketing {
    public class CampaignCustomerRepositoryImpl : DapperDatabaseConnection, ICampaignCustomerRepository {
        #region attributes
        private const string PARMNAME_CAMPAIGNID = "CampaignId";

        private const string SPNAME_GETALLCUSTOMES = "Marketing.GetAllCustomersByCampaign";
        #endregion

        #region ctor
        public CampaignCustomerRepositoryImpl() : base(Configuration.BEKDBConnectionString) { }
        #endregion

        #region methods        
        public List<CampaignCustomer> GetAllCustomersByCampaign(long campaignId) {
            return base.ReadSP<CampaignCustomer>(SPNAME_GETALLCUSTOMES, PARMNAME_CAMPAIGNID, campaignId);
        }
        #endregion
    }
}
