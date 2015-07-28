using KeithLink.Common.Core.Extensions;

using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation.SequenceComponents.ContextProviders;

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class OrganizationProfileLoader : ProfileLoaderBase {
        #region attributes
        #endregion

        #region ctor
        public OrganizationProfileLoader() {
        }
        #endregion

        #region methods
        // by default, CS Profile Sequence Components only load profiles by ID; so we have to override that functionality to get a list back
        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            CommerceModelSearch search = ((CommerceModelSearch)(queryOperation.SearchCriteria));

            if (!String.IsNullOrEmpty(search.WhereClause)) { // no search criteria, so override CS behavior to load all orgs
                CommerceResourceCollection csResources = SiteHelper.GetCsConfig();

                String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
                string fields = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), i => i.Value));
                ProfileContext ctxt = CommerceSiteContexts.Profile[SiteHelper.GetSiteName()];

                var sqlFormat = " SELECT * FROM [BEK_Commerce_profiles].[dbo].[OrganizationObject] oo {0} Order By oo.u_name OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY;";

                var pageSize = queryOperation.SearchCriteria.NumberOfItemsToReturn.HasValue ? queryOperation.SearchCriteria.NumberOfItemsToReturn.Value : int.MaxValue;
                var from = queryOperation.SearchCriteria.FirstItemIndex.HasValue ? queryOperation.SearchCriteria.FirstItemIndex.Value : 0;

                string dataSQLText = string.Format(sqlFormat, search.WhereClause.IndexOf("where", StringComparison.CurrentCultureIgnoreCase) >= 0 
                                                    ? search.WhereClause : "WHERE " + search.WhereClause, from, pageSize);
                string countSQLText = string.Format("SELECT count(*) FROM [BEK_Commerce_profiles].[dbo].[OrganizationObject] oo {0}", 
                                                    search.WhereClause.IndexOf("where", StringComparison.CurrentCultureIgnoreCase) >= 0 ? search.WhereClause : "WHERE " + search.WhereClause);


                var entities = new List<CommerceEntity>();

                using (OleDbConnection conn = new OleDbConnection(connStr)) {
                    conn.Open();

                    //Get total count
                    using (OleDbCommand cmdTotal = new OleDbCommand(countSQLText, conn)) {
                        response.TotalItemCount = (int)cmdTotal.ExecuteScalar();
                    }

                    //Read paged results
                    using (OleDbCommand cmdRead = new OleDbCommand(dataSQLText, conn)) {
                        using (OleDbDataReader dataReader = cmdRead.ExecuteReader()) {
                            while (dataReader.Read()) {
                                CommerceEntity org = new CommerceEntity("Organization");
                                
                                org.Id = dataReader.GetString("u_org_id");
                                org.SetPropertyValue("Name", dataReader.GetString("u_name"));
                                org.SetPropertyValue("CustomerNumber", dataReader.GetString("u_customer_number"));
                                org.SetPropertyValue("BranchNumber", dataReader.GetString("u_branch_number"));
                                org.SetPropertyValue("DsrNumber", dataReader.GetString("u_dsr_number"));
                                org.SetPropertyValue("ContractNumber", dataReader.GetString("u_contract_number"));
                                org.SetPropertyValue("IsPoRequired", dataReader.GetNullableBool("u_is_po_required"));
                                org.SetPropertyValue("IsPowerMenu", dataReader.GetNullableBool("u_is_power_menu"));
                                org.SetPropertyValue("OrganizationType", dataReader.GetString("u_organization_type"));
                                org.SetPropertyValue("NationalOrRegionalAccountNumber", dataReader.GetString("u_national_or_regional_account_number"));
                                org.SetPropertyValue("ParentOrganizationId", dataReader.GetString("u_parent_organization"));
                                org.SetPropertyValue("TermCode", dataReader.GetString("u_term_code"));
                                org.SetPropertyValue("CurrentBalance", dataReader.GetNullableDecimal("u_current_balance"));
                                org.SetPropertyValue("BalanceAge1", dataReader.GetNullableDecimal("u_balance_age_1"));
                                org.SetPropertyValue("BalanceAge2", dataReader.GetNullableDecimal("u_balance_age_2"));
                                org.SetPropertyValue("BalanceAge3", dataReader.GetNullableDecimal("u_balance_age_3"));
                                org.SetPropertyValue("BalanceAge4", dataReader.GetNullableDecimal("u_balance_age_4"));
                                org.SetPropertyValue("AmountDue", dataReader.GetNullableDecimal("u_amount_due"));
                                org.SetPropertyValue("AchType", dataReader.GetString("u_customer_ach_type"));
                                org.SetPropertyValue("DsmNumber", dataReader.GetString("u_dsm_number"));
                                org.SetPropertyValue("NationalId", dataReader.GetString("u_national_id"));
                                org.SetPropertyValue("NationalNumber", dataReader.GetString("u_national_number"));
                                org.SetPropertyValue("NationalSubNumber", dataReader.GetString("u_national_sub_number"));
                                org.SetPropertyValue("RegionalId", dataReader.GetString("u_regional_id"));
                                org.SetPropertyValue("RegionalNumber", dataReader.GetString("u_regional_number"));
                                org.SetPropertyValue("IsKeithnetCustomer", dataReader.GetString("u_is_keithnet_customer"));
                                org.SetPropertyValue("GeneralInfo.preferred_address", dataReader.GetString("u_preferred_address"));

                                response.CommerceEntities.Add(org);
                            }
                        }
                    }

                    if (conn.State == System.Data.ConnectionState.Open) { conn.Close(); }
                }
            } else {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }
        #endregion

        #region properties
        protected override string ProfileModelName {
            get {
                return "Organization";
            }
        }
        #endregion
    }
}