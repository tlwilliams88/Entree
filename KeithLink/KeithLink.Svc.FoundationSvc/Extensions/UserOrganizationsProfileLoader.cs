using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation.SequenceComponents.Utility;
using CommerceServer.Foundation.SequenceComponents.ContextProviders;
using CommerceServer.Foundation.SequenceComponents.CSHelpers;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class UserOrganizationsProfileLoader : ProfileLoaderBase
    {
       protected override string ProfileModelName
        {
            get
            {
                return "UserOrganizations";
            }
        }

        public UserOrganizationsProfileLoader()
        {
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            CommerceModelSearch searchCriteria = queryOperation.GetSearchCriteria<CommerceModelSearch>();
            ParameterChecker.CheckForNull(searchCriteria, "searchCriteria");

            CommercePropertyItem item = searchCriteria.Model.Properties[0];
            CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources = 
                new CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection(GetSiteName());
            String connStr = csResources ["Biz Data Service"] ["s_BizDataStoreConnectionString"].ToString( ) ;
            ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr))
            {
                conn.Open();
                using (System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(
                    @"SELECT u_user_org_id,u_user_id,oo.u_org_id,u_Name,u_trading_partner_number
                        FROM UserOrganizationObject uoo inner join OrganizationObject oo 
                        on uoo.u_org_id=oo.u_org_id WHERE uoo.u_user_id = '" + item.Value + "'",
                    conn))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    adapter.Fill(dt);

                    foreach (System.Data.DataRow r in dt.Rows)
                    {
                        CommerceEntity org = new CommerceEntity("Organization");
                        org.Id = (string)r["u_org_id"];
                        org.SetPropertyValue("GeneralInfo.Name", (string)r["u_Name"]);
                        org.SetPropertyValue("GeneralInfo.TradingPartnerNumber", (string)r["u_trading_partner_number"]);
                        response.CommerceEntities.Add(org);
                    }
                }
            }
        }

        private static string GetSiteName()
        {
            if (CommerceOperationContext.CurrentInstance == null)
            {
                throw CommonExceptions.MissingOperationContext();
            }
            return CommerceOperationContext.CurrentInstance.SiteName;
        }
    }
}