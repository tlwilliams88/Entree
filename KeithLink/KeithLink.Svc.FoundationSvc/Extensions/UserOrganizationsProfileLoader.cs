using KeithLink.Common.Core.Extensions;

using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation.SequenceComponents.Utility;
using CommerceServer.Foundation.SequenceComponents.ContextProviders;
using CommerceServer.Foundation.SequenceComponents.CSHelpers;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class UserOrganizationsProfileLoader : ProfileLoaderBase {
        #region attributes
        #endregion

        #region ctor
        public UserOrganizationsProfileLoader() {
        }
        #endregion

        #region methods
        public override void ExecuteCreate(CommerceCreateOperation createOperation, OperationCacheDictionary operationCache, CommerceCreateOperationResponse response) {
            ParameterChecker.CheckForNull(createOperation, "createOperation");
            ParameterChecker.CheckForNull(operationCache, "operationCache");
            ParameterChecker.CheckForNull(response, "response");

            base.ExecuteCreate(createOperation, operationCache, response);
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            CommerceModelSearch searchCriteria = queryOperation.GetSearchCriteria<CommerceModelSearch>();
            ParameterChecker.CheckForNull(searchCriteria, "searchCriteria");

            if (searchCriteria.Model.Properties.Count == 1) {
                string sproc = string.Empty;
                
                if (searchCriteria.Model.Properties[0].Key == "OrganizationId") { // looking for users associated to org
                    sproc = "[dbo].[sp_BEK_ReadUsersForOrg]";
                } else if (searchCriteria.Model.Properties[0].Key == "UserId") {// looking for orgs associated to user
                    sproc = "[dbo].[sp_BEK_ReadOrgsForUser]";
                }

                CommerceResourceCollection csResources = SiteHelper.GetCsConfig();
                String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];

                using (OleDbConnection conn = new OleDbConnection(connStr)) {
                    conn.Open();

                    CommercePropertyItem item = searchCriteria.Model.Properties[0];
                    
                    OleDbCommand cmd = new OleDbCommand(sproc, conn);

                    cmd.Parameters.Add(new OleDbParameter("@id", OleDbType.VarChar, 50));
                    cmd.Parameters[0].Value = item.Value;
                    cmd.Parameters[0].Direction = ParameterDirection.Input;
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd)) {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        foreach (DataRow r in dt.Rows) {
                            if (searchCriteria.Model.Properties[0].Key == "UserId") { // todo: use profile entity mapping to fill this in
                                //this.Metadata.ProfileMapping;
                                CommerceEntity org = new CommerceEntity("Organization");

                                org.Id = r.GetString("u_org_id");
                                org.SetPropertyValue("Name", r.GetString("u_name"));
                                org.SetPropertyValue("CustomerNumber", r.GetString("u_customer_number"));
                                org.SetPropertyValue("BranchNumber", r.GetString("u_branch_number"));
                                org.SetPropertyValue("DsrNumber", r.GetString("u_dsr_number"));
                                org.SetPropertyValue("ContractNumber", r.GetString("u_contract_number"));
                                org.SetPropertyValue("IsPoRequired", r.GetNullableBool("u_is_po_required"));
                                org.SetPropertyValue("IsPowerMenu", r.GetNullableBool("u_is_power_menu"));
                                org.SetPropertyValue("OrganizationType", r.GetString("u_organization_type"));
                                org.SetPropertyValue("NationalOrRegionalAccountNumber", r.GetString("u_national_or_regional_account_number"));
                                org.SetPropertyValue("ParentOrganizationId", r.GetString("u_parent_organization"));
                                org.SetPropertyValue("TermCode", r.GetString("u_term_code"));
                                org.SetPropertyValue("CurrentBalance", r.GetNullableDecimal("u_current_balance"));
                                org.SetPropertyValue("BalanceAge1", r.GetNullableDecimal("u_balance_age_1"));
                                org.SetPropertyValue("BalanceAge2", r.GetNullableDecimal("u_balance_age_2"));
                                org.SetPropertyValue("BalanceAge3", r.GetNullableDecimal("u_balance_age_3"));
                                org.SetPropertyValue("BalanceAge4", r.GetNullableDecimal("u_balance_age_4"));
                                org.SetPropertyValue("AmountDue", r.GetNullableDecimal("u_amount_due"));
                                org.SetPropertyValue("AchType", r.GetString("u_customer_ach_type"));
                                org.SetPropertyValue("GeneralInfo.preferred_address", r.GetString("u_preferred_address"));

                                response.CommerceEntities.Add(org);
                            } else if (searchCriteria.Model.Properties[0].Key == "OrganizationId") {
                                CommerceEntity org = new CommerceEntity("UserProfile");

                                org.Id = r.GetString("u_user_id");
                                org.SetPropertyValue("FirstName", r.GetString("u_first_name"));
                                org.SetPropertyValue("LastName", r.GetString("u_last_name"));
                                org.SetPropertyValue("Email", r.GetString("u_email_address"));

                                response.CommerceEntities.Add(org);
                            }
                        }
                    }

                    if (conn.State == ConnectionState.Open) { conn.Close(); }
                }
            } else {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }
        #endregion

        #region properties
        protected override string ProfileModelName {
            get {
                return "UserOrganizations";
            }
        }
        #endregion
    }
}