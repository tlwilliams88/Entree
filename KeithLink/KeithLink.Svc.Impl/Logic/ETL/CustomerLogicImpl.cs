// Commerce Server
using CommerceServer.Core.Profiles;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Core.Runtime;
using CommerceServer.Core.Shared;
using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Diagnostics;

// KeithLink
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Customers.EF;

// Core
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace KeithLink.Svc.Impl.ETL
{
    public class CustomerLogicImpl : ICustomerLogic {
        #region attributes

        private IStagingRepository stagingRepository;
        private IItemHistoryRepository _itemHistoryRepository;
        private IDsrLogic dsrLogic;
        static ProfileContext profileSystem = null;
        private readonly IEventLogRepository eventLog;

        #endregion

        #region constructor 

        public CustomerLogicImpl(IStagingRepository stagingRepository, IDsrLogic dsrLogic, IEventLogRepository eventLog, IItemHistoryRepository itemHistoryRepository)
        {
            this.stagingRepository = stagingRepository;
            this.dsrLogic = dsrLogic;
            this.eventLog = eventLog;
            this._itemHistoryRepository = itemHistoryRepository;
        }

        #endregion

        #region methods

        /// <summary>
        /// Import customer item history
        /// </summary>
        public void ImportCustomerItemHistory()
        {
            try
            {
                DateTime start = DateTime.Now;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import customer item history {0}", start.ToString()));

                // Execute processing for 8 week average items
                stagingRepository.ProcessItemHistoryData(8);

                TimeSpan took = DateTime.Now - start;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import customer item history.  Process took {0}", took.ToString()));
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog(String.Format("ETL: Error Importing customer item history -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Import customers to organization profile
        /// </summary>
        public void ImportCustomersToOrganizationProfile()
        {
            try
            {
                DateTime start = DateTime.Now;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import customers to CS {0}", start.ToString()));

                DataTable customers = stagingRepository.ReadCustomers();

                BlockingCollection<Organization> orgsForImport = new BlockingCollection<Organization>();
                BlockingCollection<AddressProfiles> addressesForImport = new BlockingCollection<AddressProfiles>();

                //Parallel.ForEach(customers.AsEnumerable(), row =>
                foreach (DataRow row in customers.Rows)
                {
                    orgsForImport.Add(CreateOrganizationFromStagedData(row));
                    addressesForImport.Add(CreateAddressFromStagedData(row));
                }

                // Get Existing Organizations from CS
                List<Organization> existingOrgs = GetExistingOrganizations(""); // for merge purposes, only pull customer_number, org_type and natl_or_regl_account_number

                ProfileContext ctxt = GetProfileContext();

                //Parallel.ForEach(orgsForImport, org =>
                foreach (Organization org in orgsForImport)
                {
                    try
                    {
                        // Create a new profile object.
                        Profile prof = null;
                        if (existingOrgs.Any(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber))
                        {
                            prof = ctxt.GetProfile(existingOrgs.Where(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber).FirstOrDefault().Id, "Organization");
                        }
                        else
                        {
                            prof = ctxt.CreateProfile((Guid.NewGuid()).ToCommerceServerFormat(), "Organization");
                        }
                        // Set the profile properties.
                        prof.Properties["GeneralInfo.name"].Value = org.Name;
                        prof.Properties["GeneralInfo.customer_number"].Value = org.CustomerNumber;
                        prof.Properties["GeneralInfo.is_po_required"].Value = org.IsPoRequired;
                        prof.Properties["GeneralInfo.is_power_menu"].Value = org.IsPowerMenu;
                        prof.Properties["GeneralInfo.contract_number"].Value = org.ContractNumber;
                        prof.Properties["GeneralInfo.dsr_number"].Value = org.DsrNumber;
                        prof.Properties["GeneralInfo.dsm_number"].Value = org.DsmNumber;
                        prof.Properties["GeneralInfo.natl_or_regl_account_number"].Value = org.NationalOrRegionalAccountNumber;
                        prof.Properties["GeneralInfo.branch_number"].Value = org.BranchNumber;
                        prof.Properties["GeneralInfo.organization_type"].Value = "0"; // customer org.OrganizationType;
                        prof.Properties["GeneralInfo.term_code"].Value = org.TermCode;
                        prof.Properties["GeneralInfo.amount_due"].Value = org.AmountDue;
                        prof.Properties["GeneralInfo.credit_limit"].Value = org.CreditLimit;
                        prof.Properties["GeneralInfo.credit_hold_flag"].Value = org.CreditHoldFlag;
                        prof.Properties["GeneralInfo.date_of_last_payment"].Value = org.DateOfLastPayment;
                        prof.Properties["GeneralInfo.current_balance"].Value = org.CurrentBalance;
                        prof.Properties["GeneralInfo.balance_age_1"].Value = org.BalanceAge1;
                        prof.Properties["GeneralInfo.balance_age_2"].Value = org.BalanceAge2;
                        prof.Properties["GeneralInfo.balance_age_3"].Value = org.BalanceAge3;
                        prof.Properties["GeneralInfo.balance_age_4"].Value = org.BalanceAge4;
                        prof.Properties["GeneralInfo.customer_ach_type"].Value = org.AchType;
                        prof.Properties["GeneralInfo.national_id"].Value = org.NationalId;
                        prof.Properties["GeneralInfo.national_number"].Value = org.NationalNumber;
                        prof.Properties["GeneralInfo.national_sub_number"].Value = org.NationalSubNumber;
                        prof.Properties["GeneralInfo.regional_id"].Value = org.RegionalId;
                        prof.Properties["GeneralInfo.regional_number"].Value = org.RegionalNumber;
                        prof.Properties["GeneralInfo.is_keithnet_customer"].Value = org.IsKeithnetCustomer;

                        Profile addressProfile = null;
                        if (prof.Properties["GeneralInfo.preferred_address"] == null || String.IsNullOrEmpty((string)prof.Properties["GeneralInfo.preferred_address"].Value))
                        { // create a new address
                            string newAddressId = (Guid.NewGuid()).ToCommerceServerFormat();
                            addressProfile = ctxt.CreateProfile(newAddressId, "Address");
                            prof.Properties["GeneralInfo.preferred_address"].Value = newAddressId;
                        }
                        else
                        { // update existing address
                            addressProfile = ctxt.GetProfile((string)prof.Properties["GeneralInfo.preferred_address"].Value, "Address");
                        }

                        AddressProfiles address = addressesForImport.Where(x => x.Description == org.CustomerNumber && x.AddressType == org.BranchNumber).FirstOrDefault();
                        addressProfile.Properties["GeneralInfo.address_name"].Value = address.AddressName;
                        addressProfile.Properties["GeneralInfo.address_line1"].Value = address.Line1;
                        addressProfile.Properties["GeneralInfo.address_line2"].Value = address.Line2;
                        addressProfile.Properties["GeneralInfo.city"].Value = address.City;
                        addressProfile.Properties["GeneralInfo.region_code"].Value = address.StateProvinceCode;
                        addressProfile.Properties["GeneralInfo.postal_code"].Value = address.ZipPostalCode;
                        addressProfile.Properties["GeneralInfo.tel_number"].Value = address.Telephone;
                        addressProfile.Update();

                        prof.Update();
                    }
                    catch (Exception ex)
                    {
                        eventLog.WriteErrorLog(String.Format("ETL: Error Importing customer to CS -- individual customer.  {0} -- {1}", ex.Message, ex.StackTrace));
                    }

                }

                TimeSpan took = DateTime.Now - start;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import customers to CS.  Process took {0}", took.ToString()));
            }
            catch (Exception e)
            {
                eventLog.WriteErrorLog(String.Format("ETL: Error Importing customers to CS -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
            }
        }

        /// <summary>
        /// Import DSR data
        /// </summary>
        public void ImportDsrInfo()
        {
            try
            {
                DateTime start = DateTime.Now;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import dsrs to CS {0}", start.ToString()));

                DataTable dsrInfo = stagingRepository.ReadDsrInfo();

                foreach (DataRow row in dsrInfo.Rows)
                {
                    try
                    {
                        var newDsr = new KeithLink.Svc.Core.Models.Profile.Dsr();
                        newDsr.Branch = row.GetString("BranchId");
                        newDsr.DsrNumber = row.GetString("DsrNumber");
                        newDsr.EmailAddress = row.GetString("EmailAddress");
                        newDsr.Name = row.GetString("Name");
                        newDsr.ImageUrl = row.GetString("ImageUrl");
                        newDsr.PhoneNumber = row.GetString("Phone");

                        dsrLogic.CreateOrUpdateDsr(newDsr);
                    }
                    catch (Exception ex1)
                    {
                        eventLog.WriteErrorLog(String.Format("ETL: Error importing dsr to CS.  {0} -- {1}", ex1.Message, ex1.StackTrace));
                    }
                }

                //TODO: Move image to multidocs
                //dsrInfo contains fields:  EmailAddress and EmployeePhoto
                DataTable dsrImages = stagingRepository.ReadDsrImages();
                foreach (DataRow row in dsrImages.Rows)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(row["EmployeePhoto"].ToString()))
                        {
                            dsrLogic.SendImageToMultiDocs(row.GetString("EmailAddress"), (byte[])row["EmployeePhoto"]);
                        }
                    }
                    catch (Exception ex2)
                    {
                        eventLog.WriteErrorLog(String.Format("ETL: Error sending dsr image to multi-docs.  {0} -- {1}", ex2.Message, ex2.StackTrace));
                    }
                }

                TimeSpan took = DateTime.Now - start;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import dsrs.  Process took {0}", took.ToString()));
            }
            catch (Exception e)
            {
                eventLog.WriteErrorLog(String.Format("ETL: Error Importing dsrs -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
            }
        }

        /// <summary>
        /// Create address from stage data
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private AddressProfiles CreateAddressFromStagedData(DataRow row)
        {
            return new AddressProfiles()
            {
                Description = row.GetString("CustomerNumber"), // use the customer number in description to look up the addy when creating the profile
				AddressType = row.GetString("BranchNumber"), //Use AddressType for branch Id for look up when addy is created
                AddressName = "Preferred",
                Line1 = row.GetString("Address1"),
                Line2 = row.GetString("Address2"),
                City = row.GetString("City"),
                StateProvinceCode = row.GetString("State"),
                ZipPostalCode = row.GetString("ZipCode"),
                Telephone = row.GetString("Telephone")
				
            };
        }

        /// <summary>
        /// Create organization from staged data
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Organization CreateOrganizationFromStagedData(DataRow row)
        {
            Organization org = new Organization()
            {
                Name = row.GetString("CustomerName"),
                CustomerNumber = row.GetString("CustomerNumber"),
                BranchNumber = row.GetString("BranchNumber"),
                DsrNumber = row.GetString("DsrNumber"),
                DsmNumber = row.GetString("DsmNumber"),
                NationalOrRegionalAccountNumber = row.GetString("NationalOrRegionalAccountNumber"),
                ContractNumber = row.GetString("ContractNumber"),
                IsPoRequired = GetBoolFromYorN(row.GetString("PORequiredFlag")),
                IsPowerMenu = GetBoolFromYorN(row.GetString("PowerMenu")),
                TermCode = row.GetString("TermCode"),
                CreditLimit = row.GetNullableDecimal("CreditLimit"),
                CreditHoldFlag = row.GetString("CreditHoldFlag"),
                DateOfLastPayment = row.GetNullableDateTime("DateOfLastPayment", "yyyyMMdd"),
                AmountDue = row.GetNullableDecimal("AmountDue"),
                CurrentBalance = row.GetNullableDecimal("CurrentBalance"),
                BalanceAge1 = row.GetNullableDecimal("BalanceAge1"),
                BalanceAge2 = row.GetNullableDecimal("BalanceAge2"),
                BalanceAge3 = row.GetNullableDecimal("BalanceAge3"),
                BalanceAge4 = row.GetNullableDecimal("BalanceAge4"),
                AchType = row.GetString("AchType"),
                NationalId = row.GetString("NationalId"),
                NationalNumber = row.GetString("NationalNumber"),
                NationalSubNumber = row.GetString("NationalSubNumber"),
                RegionalId = row.GetString("RegionalId"),
                RegionalNumber = row.GetString("RegionalNumber"),
                IsKeithnetCustomer = row.GetString("IsKeithnetCustomer")
                
                // NationalAccountId = row.Get // this will come from a separate file
                // TODO, add address info
            };
            return org;
        }

        /// <summary>
        /// Helper function to convert Y/N to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool GetBoolFromYorN(string value)
        {
            // if we have a value and it is Y, return true
            return (!(String.IsNullOrEmpty(value)) && value.Equals("Y", StringComparison.CurrentCulture));
        }

        /// <summary>
        /// Get existing organizations
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        private List<Organization> GetExistingOrganizations(string organizationType)
        {
            ProfileContext ctxt = GetProfileContext();
            string cmdText = "SELECT GeneralInfo.org_id,GeneralInfo.natl_or_regl_account_number,GeneralInfo.customer_number,GeneralInfo.organization_type, GeneralInfo.branch_number FROM Organization";

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
                    existingOrgs.Add(new Organization() { CustomerNumber = rs.Fields["GeneralInfo.customer_number"].Value.ToString(),
                                                          NationalOrRegionalAccountNumber = rs.Fields["GeneralInfo.natl_or_regl_account_number"].Value.ToString(),
                                                          OrganizationType = rs.Fields["GeneralInfo.organization_type"].Value.ToString(),
														  Id = rs.Fields["GeneralInfo.org_id"].Value.ToString(),
														  BranchNumber = rs.Fields["GeneralInfo.branch_number"].Value.ToString()
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

        /// <summary>
        /// Get profile context
        /// </summary>
        /// <returns></returns>
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

        #endregion
    }
}
