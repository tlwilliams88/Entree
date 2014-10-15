using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core.Extensions;
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

        public override void ExecuteCreate(CommerceCreateOperation createOperation, OperationCacheDictionary operationCache, CommerceCreateOperationResponse response)
        {
            ParameterChecker.CheckForNull(createOperation, "createOperation");
            ParameterChecker.CheckForNull(operationCache, "operationCache");
            ParameterChecker.CheckForNull(response, "response");

            base.ExecuteCreate(createOperation, operationCache, response);
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            CommerceModelSearch searchCriteria = queryOperation.GetSearchCriteria<CommerceModelSearch>();
            ParameterChecker.CheckForNull(searchCriteria, "searchCriteria");

            if (searchCriteria.Model.Properties.Count == 1)
            {
                string query = string.Empty;
                CommercePropertyItem item = searchCriteria.Model.Properties[0];
                if (searchCriteria.Model.Properties[0].Key == "OrganizationId") // looking for users associated to org
                    query = @"SELECT uoo.u_user_org_id,uoo.u_org_id,uo.u_user_id,uo.u_first_name,uo.u_last_name,uo.u_email_address
                        FROM UserOrganizationObject uoo inner join UserObject uo 
                        on uoo.u_user_id=uo.u_user_id WHERE uoo.u_org_id = '" + item.Value + "'";
                else if (searchCriteria.Model.Properties[0].Key == "UserId") // looking for orgs associated to user
                    query = @"SELECT u_user_org_id,u_user_id,oo.u_org_id,u_Name,u_customer_number,u_branch_number,u_contract_number,u_dsr_number,
                        	   u_is_po_required,u_is_power_menu,u_organization_type,u_national_or_regional_account_number,u_parent_organization
                                FROM UserOrganizationObject uoo inner join OrganizationObject oo on uoo.u_org_id=oo.u_org_id WHERE uoo.u_user_id = '" + item.Value + "'";

                CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources =
                    new CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection(SiteHelper.GetSiteName());
                String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
                using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr))
                {
                    conn.Open();
                    using (System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(query, conn))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        adapter.Fill(dt);

                        foreach (System.Data.DataRow r in dt.Rows)
                        {
                            if (searchCriteria.Model.Properties[0].Key == "UserId")
                            { // todo: use profile entity mapping to fill this in
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
                                response.CommerceEntities.Add(org);
                            }
                            else if (searchCriteria.Model.Properties[0].Key == "OrganizationId")
                            {
                                CommerceEntity org = new CommerceEntity("UserProfile");
                                org.Id = r.GetString("u_org_id");
                                org.SetPropertyValue("FirstName", r.GetString("u_first_name"));
                                org.SetPropertyValue("LastName", r.GetString("u_last_name"));
                                org.SetPropertyValue("Email", r.GetString("u_email_address"));
                                response.CommerceEntities.Add(org);
                            }
                        }
                    }
                }
            }
            else
            {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }
    }
}