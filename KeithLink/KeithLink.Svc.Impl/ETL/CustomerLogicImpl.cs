using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Concurrent;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using CommerceServer.Core.Profiles;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Core.Runtime;
using CommerceServer.Core.Shared;
using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Diagnostics;

namespace KeithLink.Svc.Impl.ETL
{
    public class CustomerLogicImpl : ICustomerLogic
    {
        private IStagingRepository stagingRepository;
        static ProfileContext profileSystem = null;

        public CustomerLogicImpl(IStagingRepository stagingRepository)
        {
            this.stagingRepository = stagingRepository;
        }

        public void ImportCustomersToOrganizationProfile()
        {
            DateTime start = DateTime.Now;
            DataTable customers = stagingRepository.ReadCustomers();

            BlockingCollection<Organization> orgsForImport = new BlockingCollection<Organization>();
            BlockingCollection<Organization> errorOrgs = new BlockingCollection<Organization>();
            BlockingCollection<string> orgAddOrUpdate = new BlockingCollection<string>();

            Parallel.ForEach(customers.AsEnumerable(), row =>
            {
                orgsForImport.Add(CreateOrganization(row));
            });

            // Get Existing Organizations from CS
            List<Organization> existingOrgs = GetExistingOrganizations(""); // for merge purposes, only pull customer_number, org_type and natl_or_regl_account_number

            ProfileContext ctxt = GetProfileContext();
            
            Parallel.ForEach(orgsForImport, org =>
                {
                    string addUpdate = "";
                    try
                    {
                        // Create a new profile object.
                        Profile prof = null;
                        if (existingOrgs.Any(x => x.GeneralInfocustomerNumber == org.GeneralInfocustomerNumber))
                        {
                            addUpdate = "update";
                            prof = ctxt.GetProfile(existingOrgs.Where(x => x.GeneralInfocustomerNumber == org.GeneralInfocustomerNumber).FirstOrDefault().Id, "Organization");
                        }
                        else
                        {
                            addUpdate = "add";
                            prof = ctxt.CreateProfile((Guid.NewGuid()).ToCommerceServerFormat(), "Organization");
                        }
                        // Set the profile properties.
                        prof.Properties["GeneralInfo.name"].Value = org.GeneralInfoname;
                        prof.Properties["GeneralInfo.customer_number"].Value = org.GeneralInfocustomerNumber;
                        prof.Properties["GeneralInfo.is_po_required"].Value = org.GeneralInfoisPoRequired;
                        prof.Properties["GeneralInfo.is_power_menu"].Value = org.GeneralInfoisPowerMenu;
                        prof.Properties["GeneralInfo.contract_number"].Value = org.GeneralInfocontractNumber;
                        prof.Properties["GeneralInfo.dsr_number"].Value = org.GeneralInfodsrNumber;
                        prof.Properties["GeneralInfo.natl_or_regl_account_number"].Value = org.GeneralInfonatlOrReglAccountNumber;
                        prof.Properties["GeneralInfo.branch_number"].Value = org.GeneralInfobranchNumber;
                        prof.Properties["GeneralInfo.organization_type"].Value = "0"; // customer org.GeneralInfoorganizationType;
                        // prof.Properties["GeneralInfo.national_account_id"].Value = ; // TODO - not available in current data feeds

                        // Update the profile with the property values.
                        prof.Update();
                    }
                    catch (Exception ex)
                    {
                        errorOrgs.Add(org);
                        orgAddOrUpdate.Add(addUpdate);
                    }
                });

            TimeSpan took = DateTime.Now - start;
            return;
        }

        private Organization CreateOrganization(DataRow row)
        {
            Organization org = new Organization()
            {
                GeneralInfoname = row.GetString("CustomerName"),
                GeneralInfocustomerNumber = row.GetString("CustomerNumber"),
                GeneralInfobranchNumber = row.GetString("BranchNumber"),
                GeneralInfodsrNumber = row.GetString("DsrNumber"),
                GeneralInfonatlOrReglAccountNumber = row.GetString("NationalOrRegionalAccountNumber"),
                GeneralInfocontractNumber = row.GetString("ContractNumber"),
                GeneralInfoisPoRequired = GetBoolFromYorN(row.GetString("PORequiredFlag")),
                GeneralInfoisPowerMenu = GetBoolFromYorN(row.GetString("PowerMenu"))
                // NationalAccountId = row.Get // TODO - this will come from a separate file
            };
            return org;
        }

        private static bool GetBoolFromYorN(string value)
        {
            // if we have a value and it is Y, return true
            return (!(String.IsNullOrEmpty(value)) && value.Equals("Y", StringComparison.CurrentCulture));
        }

        private List<Organization> GetExistingOrganizations(string organizationType)
        {
            ProfileContext ctxt = GetProfileContext();
            string cmdText = "SELECT GeneralInfo.org_id,GeneralInfo.natl_or_regl_account_number,GeneralInfo.customer_number,GeneralInfo.organization_type FROM Organization";

            // Create a new RecordsetClass object.
            ADODB.Recordset rs = new ADODB.Recordset();
            List<Organization> existingOrgs = new List<Organization>();

            try
            {
                // Open a RecordsetClass instance by executing the SQL statement to the CSOLEDB provider.
                rs.Open(
                cmdText,
                ctxt.CommerceOleDbProvider,
                ADODB.CursorTypeEnum.adOpenForwardOnly,
                ADODB.LockTypeEnum.adLockReadOnly,
                (int)ADODB.CommandTypeEnum.adCmdText);

                // Iterate through the records.
                while (!rs.EOF)
                {
                    // Write out the user_id value.
                    existingOrgs.Add(new Organization() { GeneralInfocustomerNumber = rs.Fields["GeneralInfo.customer_number"].Value.ToString(),
                                                          GeneralInfonatlOrReglAccountNumber = rs.Fields["GeneralInfo.natl_or_regl_account_number"].Value.ToString(),
                                                          GeneralInfoorganizationType = rs.Fields["GeneralInfo.organization_type"].Value.ToString(),
                                                          Id = rs.Fields["GeneralInfo.org_id"].Value.ToString()
                                                        });

                    // Move to the next record.
                    rs.MoveNext();
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
            return existingOrgs;
        }

        private static ProfileContext GetProfileContext()
        {
            // Get the Profiles run-time object.
            if (profileSystem == null)
            {
                CommerceResourceCollection resources = new CommerceResourceCollection(Configuration.CSSiteName);
                CommerceResource profilesResource = resources["Biz Data Service"];
                string profilesvcConnstr = profilesResource["s_ProfileServiceConnectionString"].ToString();
                string providerConnstr = profilesResource["s_CommerceProviderConnectionString"].ToString();
                string bizdataConnstr = profilesResource["s_BizDataStoreConnectionString"].ToString();
                profileSystem = new ProfileContext(profilesvcConnstr, providerConnstr, bizdataConnstr, new ConsoleDebugContext(DebugMode.Debug));
            }
            return profileSystem;
        }
    }
}
