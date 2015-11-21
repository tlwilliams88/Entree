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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

namespace KeithLink.Svc.Impl.ETL
{
    public class CustomerLogicImpl : ICustomerLogic
    {
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

        public void ImportCustomersToOrganizationProfile()
        {
            try
            {
                DateTime start = DateTime.Now;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import organizations to CS {0}", start.ToString()));

                stagingRepository.ImportCustomersToCS();

                TimeSpan took = DateTime.Now - start;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import organizations to CS.  Process took {0}", took.ToString()));
            }
            catch (Exception ex)
            {
                eventLog.WriteErrorLog(String.Format("ETL: Error importing organizations to CS -- whole process failed.  {0} -- {1}", ex.Message, ex.StackTrace));
            }
        }

        /*
         * deprecated 2015-09-08, JMM -- moved all processing into SQL
        /// <summary>
        /// Import Customers to CS
        /// </summary>
        public void ImportCustomersToOrganizationProfile()
        {
            try
            {
                DateTime start = DateTime.Now;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Starting:  Import customers to CS {0}", start.ToString()));

                DataTable customers = stagingRepository.ReadCustomers();

                List<Organization> orgsForImport = new List<Organization>();
                List<AddressProfiles> addressesForImport = new List<AddressProfiles>();

                foreach (DataRow row in customers.Rows)
                {
                    orgsForImport.Add(CreateOrganizationFromStagedData(row));
                    addressesForImport.Add(CreateAddressFromStagedData(row));
                }

                // Get Existing Organizations from CS
                List<Organization> existingOrgs = GetExistingOrganizations(""); // for merge purposes, only pull customer_number, org_type, preferred_address and natl_or_regl_account_number

                foreach (Organization org in orgsForImport)
                //Parallel.ForEach(orgsForImport, new ParallelOptions { MaxDegreeOfParallelism = 2 }, org =>
                {
                    Hashtable orgValues = null;
                    Hashtable addressValues = null;
                    try
                    {
                        //if org exists, then update it
                        if (existingOrgs.Any(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber))
                        {
                            org.Id = existingOrgs.Where(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber).FirstOrDefault().Id;
                            orgValues = GetUpdateOrganizationStatement(org);
                            stagingRepository.ExecuteProfileObjectQuery(orgValues["Query"].ToString());
                        }
                        //if it doesn't exist, then create it
                        else
                        {
                            orgValues = GetCreateOrganizationStatment(org);
                            stagingRepository.ExecuteProfileObjectQuery(orgValues["Query"].ToString());
                        }

                        AddressProfiles address = addressesForImport.Where(x => x.Description == org.CustomerNumber && x.AddressType == org.BranchNumber).FirstOrDefault();
                        string existingOrgId = String.Empty;
                        if (existingOrgs.Any(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber))
                        {
                            existingOrgId = existingOrgs.Where(x => x.CustomerNumber == org.CustomerNumber && x.BranchNumber == org.BranchNumber).FirstOrDefault().Id;
                        }

                        DataTable results = GetSingleProfileProperty(ProfileObjectType.Organization, "u_preferred_address", existingOrgId);

                        if (results.Rows.Count > 0)
                        {
                            address.Id = results.Rows[0]["u_preferred_address"].ToString();
                            addressValues = GetUpdateAddressStatement(address);
                            stagingRepository.ExecuteProfileObjectQuery(addressValues["Query"].ToString());

                        }
                        else
                        {
                            addressValues = GetCreateAddressStatement(address);
                            stagingRepository.ExecuteProfileObjectQuery(addressValues["Query"].ToString());

                            //update the preferred address of parent object
                            if (orgValues != null)
                            {
                                UpdateSingleProfileProperty(ProfileObjectType.Organization, "u_preferred_address", addressValues["AddressId"].ToString(), orgValues["OrganizationId"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        eventLog.WriteErrorLog(String.Format("ETL: Error Importing customer to CS -- individual customer.  {0} -- {1}", ex.Message, ex.StackTrace));
                    }
                }
                //);

                TimeSpan took = DateTime.Now - start;
                eventLog.WriteInformationLog(String.Format("ETL: Import Process Finished:  Import customers to CS.  Process took {0}", took.ToString()));
            }
            catch (Exception e)
            {
                eventLog.WriteErrorLog(String.Format("ETL: Error Importing customers to CS -- whole process failed.  {0} -- {1}", e.Message, e.StackTrace));
            }
        }
        */
        /// <summary>
        /// Update a single profile property
        /// </summary>
        private void UpdateSingleProfileProperty(ProfileObjectType objectType, string propertyName, string propertyValue, string objectId)
        {
            switch (objectType)
            {
                case ProfileObjectType.Organization:
                    stagingRepository.ExecuteProfileObjectQuery(String.Format("UPDATE OrganizationObject SET {0} = '{1}' WHERE u_org_id = '{2}'", propertyName, propertyValue, objectId));
                    break;
                case ProfileObjectType.Address:
                    stagingRepository.ExecuteProfileObjectQuery(String.Format("UPDATE Addresses SET {0} = '{1}' WHERE u_org_id = '{2}'", propertyName, propertyValue, objectId));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Retrieve a single profile property
        /// </summary>
        private DataTable GetSingleProfileProperty(ProfileObjectType objectType, string propertyName, string objectId)
        {
            switch (objectType)
            {
                case ProfileObjectType.Organization:
                    return stagingRepository.ExecuteProfileObjectQueryReturn(String.Format("SELECT {0} FROM OrganizationObject WHERE u_org_id = '{1}'", propertyName, objectId));
                case ProfileObjectType.Address:
                    return stagingRepository.ExecuteProfileObjectQueryReturn(String.Format("SELECT {0} FROM Addresses WHERE u_org_id = '{1}'", propertyName, objectId));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Generate SQL update query for organizations
        /// </summary>
        private Hashtable GetUpdateOrganizationStatement(Organization org)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE OrganizationObject SET ");

            Hashtable values = SetOrganizationProperties(org, ProfileOperationType.Update);
            query.Append(values["Query"]).ToString();
            query.Append(String.Format(" WHERE [u_org_id] = '{0}'", org.Id));

            returnVal.Add("Query", query.ToString());
            returnVal.Add("OrganizationId", org.Id);

            return returnVal;
        }

        /// <summary>
        /// Generate SQL create query for organizations
        /// </summary>
        private Hashtable GetCreateOrganizationStatment(Organization org)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            query.Append(@"INSERT INTO OrganizationObject 
                            (
                                [u_org_id]
                                , [u_Name]
                                , [u_is_po_required]
                                , [u_is_power_menu]
                                , [u_contract_number]
                                , [u_dsr_number]
                                , [u_national_or_regional_account_number]
                                , [u_branch_number]
                                , [u_customer_number]
                                , [u_parent_organization]
                                , [u_organization_type]
                                , [u_national_account_id]
                                , [u_term_code]
                                , [u_credit_limit]
                                , [u_credit_hold_flag]
                                , [u_date_of_last_payment]
                                , [u_amount_due]
                                , [u_current_balance]
                                , [u_balance_age_1]
                                , [u_balance_age_2]
                                , [u_balance_age_3]
                                , [u_balance_age_4]
                                , [u_customer_ach_type]
                                , [u_dsm_number]
                                , [u_national_id]
                                , [u_national_number]
                                , [u_national_sub_number]
                                , [u_regional_id]
                                , [u_regional_number]
                                , [u_is_keithnet_customer]
                                , [u_national_id_desc]
                                , [u_national_numbersub_desc]
                                , [u_regional_id_desc]
                                , [u_regional_number_desc]
                           )
                            VALUES ( ");

            Hashtable values = SetOrganizationProperties(org, ProfileOperationType.Create);
            query.Append(values["Query"].ToString());
            query.Append(")");

            returnVal.Add("Query", query.ToString());
            returnVal.Add("OrganizationId", values["OrganizationId"]);

            return returnVal;
        }

        /// <summary>
        /// Set the individual properties for use in query generation
        /// </summary>
        private Hashtable SetOrganizationProperties(Organization org, ProfileOperationType operation)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            // Set the profile properties.
            string newId = Guid.NewGuid().ToCommerceServerFormat();
            if (operation == ProfileOperationType.Create)
            {
                query.Append(String.Format("'{0}'", newId));
                if (org.Name != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.Name))); else query.Append(",null");
                if (org.IsPoRequired != null) query.Append(String.Format(",'{0}'", org.IsPoRequired)); else query.Append(",null");
                if (org.IsPowerMenu != null) query.Append(String.Format(",'{0}'", org.IsPowerMenu)); else query.Append(",null");
                if (org.ContractNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.ContractNumber))); else query.Append(",null");
                if (org.DsrNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.DsrNumber))); else query.Append(",null");
                if (org.NationalOrRegionalAccountNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalOrRegionalAccountNumber))); else query.Append(",null");
                if (org.BranchNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.BranchNumber))); else query.Append(",null");
                if (org.CustomerNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.CustomerNumber))); else query.Append(",null");
                if (org.ParentOrganizationId != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.ParentOrganizationId))); else query.Append(",null");
                if (org.OrganizationType != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.OrganizationType))); else query.Append(",null");
                if (org.NationalAccountId != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalAccountId))); else query.Append(",null");
                if (org.TermCode != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.TermCode))); else query.Append(",null");
                if (org.CreditLimit != null) query.Append(String.Format(",'{0}'", org.CreditLimit)); else query.Append(",null");
                if (org.CreditHoldFlag != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.CreditHoldFlag))); else query.Append(",null");
                if (org.DateOfLastPayment != null) query.Append(String.Format(",'{0}'", org.DateOfLastPayment)); else query.Append(",null");
                if (org.AmountDue != null) query.Append(String.Format(",'{0}'", org.AmountDue)); else query.Append(",null");
                if (org.CurrentBalance != null) query.Append(String.Format(",'{0}'", org.CurrentBalance)); else query.Append(",null");
                if (org.BalanceAge1 != null) query.Append(String.Format(",'{0}'", org.BalanceAge1)); else query.Append(",null");
                if (org.BalanceAge2 != null) query.Append(String.Format(",'{0}'", org.BalanceAge2)); else query.Append(",null");
                if (org.BalanceAge3 != null) query.Append(String.Format(",'{0}'", org.BalanceAge3)); else query.Append(",null");
                if (org.BalanceAge4 != null) query.Append(String.Format(",'{0}'", org.BalanceAge4)); else query.Append(",null");
                if (org.AchType != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.AchType))); else query.Append(",null");
                if (org.DsmNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.DsmNumber))); else query.Append(",null");
                if (org.NationalId != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalId))); else query.Append(",null");
                if (org.NationalNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalNumber))); else query.Append(",null");
                if (org.NationalSubNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalSubNumber))); else query.Append(",null");
                if (org.RegionalId != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.RegionalId))); else query.Append(",null");
                if (org.RegionalNumber != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.RegionalNumber))); else query.Append(",null");
                if (org.IsKeithnetCustomer != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.IsKeithnetCustomer))); else query.Append(",null");

                if (org.NationalIdDesc != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalIdDesc))); else query.Append(",null");
                if (org.NationalNumberSubDesc != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.NationalNumberSubDesc))); else query.Append(",null");
                if (org.RegionalIdDesc != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.RegionalIdDesc))); else query.Append(",null");
                if (org.RegionalNumberDesc != null) query.Append(String.Format(",'{0}'", ToSQLFormat(org.RegionalNumberDesc))); else query.Append(",null");

                returnVal.Add("OrganizationId", newId);
            }
            else
            {
                if (org.Name != null) query.Append(String.Format("[u_Name] = '{0}'", ToSQLFormat(org.Name))); else query.Append("[u_Name] = ''"); //always attach a value to the first record in the update statement
                if (org.IsPoRequired != null) query.Append(String.Format(",[u_is_po_required] = '{0}'", org.IsPoRequired));
                if (org.IsPowerMenu != null) query.Append(String.Format(",[u_is_power_menu] = '{0}'", org.IsPowerMenu));
                if (org.ContractNumber != null) query.Append(String.Format(",[u_contract_number] = '{0}'", ToSQLFormat(org.ContractNumber)));
                if (org.DsrNumber != null) query.Append(String.Format(",[u_dsr_number] = '{0}'", ToSQLFormat(org.DsrNumber)));
                if (org.NationalOrRegionalAccountNumber != null) query.Append(String.Format(",[u_national_or_regional_account_number] = '{0}'", ToSQLFormat(org.NationalOrRegionalAccountNumber)));
                if (org.BranchNumber != null) query.Append(String.Format(",[u_branch_number] = '{0}'", ToSQLFormat(org.BranchNumber)));
                if (org.CustomerNumber != null) query.Append(String.Format(",[u_customer_number] = '{0}'", ToSQLFormat(org.CustomerNumber)));
                if (org.ParentOrganizationId != null) query.Append(String.Format(",[u_parent_organization] = '{0}'", ToSQLFormat(org.ParentOrganizationId)));
                if (org.OrganizationType != null) query.Append(String.Format(",[u_organization_type] = '{0}'", ToSQLFormat(org.OrganizationType)));
                if (org.NationalAccountId != null) query.Append(String.Format(",[u_national_account_id] = '{0}'", ToSQLFormat(org.NationalAccountId)));
                if (org.TermCode != null) query.Append(String.Format(",[u_term_code] = '{0}'", ToSQLFormat(org.TermCode)));
                if (org.CreditLimit != null) query.Append(String.Format(",[u_credit_limit] = '{0}'", org.CreditLimit));
                if (org.CreditHoldFlag != null) query.Append(String.Format(",[u_credit_hold_flag] = '{0}'", ToSQLFormat(org.CreditHoldFlag)));
                if (org.DateOfLastPayment != null) query.Append(String.Format(",[u_date_of_last_payment] = '{0}'", org.DateOfLastPayment));
                if (org.AmountDue != null) query.Append(String.Format(",[u_amount_due] = '{0}'", org.AmountDue));
                if (org.CurrentBalance != null) query.Append(String.Format(",[u_current_balance] = '{0}'", org.CurrentBalance));
                if (org.BalanceAge1 != null) query.Append(String.Format(",[u_balance_age_1] = '{0}'", org.BalanceAge1));
                if (org.BalanceAge2 != null) query.Append(String.Format(",[u_balance_age_2] = '{0}'", org.BalanceAge2));
                if (org.BalanceAge3 != null) query.Append(String.Format(",[u_balance_age_3] = '{0}'", org.BalanceAge3));
                if (org.BalanceAge4 != null) query.Append(String.Format(",[u_balance_age_4] = '{0}'", org.BalanceAge4));
                if (org.AchType != null) query.Append(String.Format(",[u_customer_ach_type] = '{0}'", ToSQLFormat(org.AchType)));
                if (org.DsmNumber != null) query.Append(String.Format(",[u_dsm_number] = '{0}'", ToSQLFormat(org.DsmNumber)));
                if (org.NationalId != null) query.Append(String.Format(",[u_national_id] = '{0}'", ToSQLFormat(org.NationalId)));
                if (org.NationalNumber != null) query.Append(String.Format(",[u_national_number] = '{0}'", ToSQLFormat(org.NationalNumber)));
                if (org.NationalSubNumber != null) query.Append(String.Format(",[u_national_sub_number] = '{0}'", ToSQLFormat(org.NationalSubNumber)));
                if (org.RegionalId != null) query.Append(String.Format(",[u_regional_id] = '{0}'", org.RegionalId));
                if (org.RegionalNumber != null) query.Append(String.Format(",[u_regional_number] = '{0}'", ToSQLFormat(org.RegionalNumber)));
                if (org.IsKeithnetCustomer != null) query.Append(String.Format(",[u_is_keithnet_customer] = '{0}'", ToSQLFormat(org.IsKeithnetCustomer)));

                if (org.NationalIdDesc != null) query.Append(String.Format(",[u_national_id_desc] = '{0}'", ToSQLFormat(org.NationalIdDesc)));
                if (org.NationalNumberSubDesc != null) query.Append(String.Format(",[u_national_numbersub_desc] = '{0}'", ToSQLFormat(org.NationalNumberSubDesc)));
                if (org.RegionalIdDesc != null) query.Append(String.Format(",[u_regional_id_desc] = '{0}'", ToSQLFormat(org.RegionalIdDesc)));
                if (org.RegionalNumberDesc != null) query.Append(String.Format(",[u_regional_number_desc] = '{0}'", ToSQLFormat(org.RegionalNumberDesc)));

                returnVal.Add("OrganizationId", org.Id);
            }

            returnVal.Add("Query", query.ToString());

            return returnVal;

        }

        /// <summary>
        /// Generate SQL create query for addresses
        /// </summary>
        private Hashtable GetCreateAddressStatement(AddressProfiles address)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            query.Append(@"INSERT INTO ADDRESSES
                (
                    [u_address_id]
                    , [u_address_name]
                    , [u_address_line1]
                    , [u_address_line2]
                    , [u_city]
                    , [u_region_code]
                    , [u_postal_code]
                    , [u_tel_number]
                )
                VALUES (");

            Hashtable values = SetAddressProperties(address, ProfileOperationType.Create);
            query.Append(values["Query"].ToString());
            query.Append(");");

            returnVal.Add("Query", query);
            returnVal.Add("AddressId", values["AddressId"]);

            return returnVal;
        }

        /// <summary>
        /// Generate SQL update query for addresses
        /// </summary>
        private Hashtable GetUpdateAddressStatement(AddressProfiles address)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE Addresses SET ");

            Hashtable values = SetAddressProperties(address, ProfileOperationType.Update);
            query.Append(values["Query"]);
            query.Append(String.Format(" WHERE [u_address_id] = '{0}'", address.Id));

            returnVal.Add("Query", query.ToString());
            returnVal.Add("AddressId", values["AddressId"].ToString());

            return returnVal;
        }

        /// <summary>
        /// Set the individual properties for use in query generation
        /// </summary>
        private Hashtable SetAddressProperties(AddressProfiles address, ProfileOperationType operation)
        {
            Hashtable returnVal = new Hashtable();
            StringBuilder query = new StringBuilder();
            string newId = Guid.NewGuid().ToCommerceServerFormat();
            // Set the profile properties.
            if (operation == ProfileOperationType.Create)
            {
                query.Append(String.Format("'{0}'", newId));
                if (address.AddressName != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.AddressName))); else query.Append(",null");
                if (address.Line1 != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.Line1))); else query.Append(",null");
                if (address.Line2 != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.Line2))); else query.Append(",null");
                if (address.City != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.City))); else query.Append(",null");
                if (address.StateProvinceCode != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.StateProvinceCode))); else query.Append(",null");
                if (address.ZipPostalCode != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.ZipPostalCode))); else query.Append(",null");
                if (address.Telephone != null) query.Append(String.Format(",'{0}'", ToSQLFormat(address.Telephone))); else query.Append(",null");

                returnVal.Add("AddressId", newId);
            }
            else
            {
                if (address.AddressName != null) query.Append(String.Format("[u_address_name] = '{0}'", ToSQLFormat(address.AddressName))); else query.Append("[u_address_name] = ''"); //always attach a value to the first record in the update statement
                if (address.Line1 != null) query.Append(String.Format(",[u_address_line1] = '{0}'", ToSQLFormat(address.Line1)));
                if (address.Line2 != null) query.Append(String.Format(",[u_address_line2] = '{0}'", ToSQLFormat(address.Line2)));
                if (address.City != null) query.Append(String.Format(",[u_city] = '{0}'", ToSQLFormat(address.City)));
                if (address.StateProvinceCode != null) query.Append(String.Format(",[u_region_code] = '{0}'", ToSQLFormat(address.StateProvinceCode)));
                if (address.ZipPostalCode != null) query.Append(String.Format(",[u_postal_code] = '{0}'", ToSQLFormat(address.ZipPostalCode)));
                if (address.Telephone != null) query.Append(String.Format(",[u_tel_number] = '{0}'", ToSQLFormat(address.Telephone)));

                returnVal.Add("AddressId", address.Id);
            }

            returnVal.Add("Query", query);

            return returnVal;
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
                IsKeithnetCustomer = row.GetString("IsKeithnetCustomer"),
                NationalIdDesc = row.GetString("NationalIdDesc"),
                NationalNumberSubDesc = row.GetString("NationalNumberAndSubDesc"),
                RegionalIdDesc = row.GetString("RegionalIdDesc"),
                RegionalNumberDesc = row.GetString("RegionalNumberDesc")
                // NationalAccountId = row.Get // this will come from a separate file
                // TODO, add address info
            };
            return org;
        }

        /// <summary>
        /// Import customer tasks
        /// </summary>
        public void ImportCustomerTasks()
        {
            try
            {
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Customers");
                var customerTask = Task.Factory.StartNew(() => ImportCustomersToOrganizationProfile());
                eventLog.WriteInformationLog("ETL Import Process Starting:  Import Dsrs");
                var dsrTask = Task.Factory.StartNew(() => ImportDsrInfo());

                Task.WaitAll(customerTask, dsrTask);
            }
            catch (Exception ex)
            {
                //log
                eventLog.WriteErrorLog("Error with ETL Import -- Import Customer Tasks", ex);
            }
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
            string cmdText = "SELECT GeneralInfo.org_id,GeneralInfo.natl_or_regl_account_number,GeneralInfo.customer_number,GeneralInfo.organization_type, GeneralInfo.branch_number, GeneralInfo.preferred_address FROM Organization";

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
                    existingOrgs.Add(new Organization()
                    {
                        CustomerNumber = rs.Fields["GeneralInfo.customer_number"].Value.ToString(),
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
        /// Get existing organizations
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        private Organization GetExistingOrganization(string customerNumber, string branchId)
        {
            ProfileContext ctxt = GetProfileContext();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("SELECT GeneralInfo.org_id,GeneralInfo.natl_or_regl_account_number,GeneralInfo.customer_number,GeneralInfo.organization_type, GeneralInfo.branch_number, GeneralInfo.is_keithnet_customer, GeneralInfo.customer_ach_type FROM Organization");
            cmdText.Append(String.Format(" WHERE GeneralInfo.customer_number = '{0}' AND GeneralInfo.branch_number = '{1}'", customerNumber, branchId));

            // Create a new RecordsetClass object.
            ADODB.Recordset rs = new ADODB.Recordset();
            List<Organization> existingOrgs = new List<Organization>();

            try
            {
                // Open a RecordsetClass instance by executing the SQL statement to the CSOLEDB provider.
                rs.Open(
                cmdText.ToString(),
                ctxt.CommerceOleDbProvider,
                ADODB.CursorTypeEnum.adOpenForwardOnly,
                ADODB.LockTypeEnum.adLockReadOnly,
                (int)ADODB.CommandTypeEnum.adCmdText);

                // Iterate through the records.
                while (!rs.EOF)
                {
                    // Write out the user_id value.
                    existingOrgs.Add(new Organization()
                    {
                        CustomerNumber = rs.Fields["GeneralInfo.customer_number"].Value.ToString(),
                        NationalOrRegionalAccountNumber = rs.Fields["GeneralInfo.natl_or_regl_account_number"].Value.ToString(),
                        OrganizationType = rs.Fields["GeneralInfo.organization_type"].Value.ToString(),
                        Id = rs.Fields["GeneralInfo.org_id"].Value.ToString(),
                        BranchNumber = rs.Fields["GeneralInfo.branch_number"].Value.ToString(),
                        IsKeithnetCustomer = rs.Fields["GeneralInfo.is_keithnet_customer"].Value.ToString(),
                        AchType = rs.Fields["GeneralInfo.customer_ach_type"].Value.ToString()
                    });

                    // Move to the next record.
                    rs.MoveNext();
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
            if (existingOrgs.Count != 1)
            {
                throw new Exception("More than one customer with a specific customer and branch combination");
            }
            else
            {
                return existingOrgs.FirstOrDefault();
            }

        }

        /// <summary>
        /// Compare data from incoming ETL file to updated profile to ensure important fields were updated
        /// </summary>
        /// <returns></returns>
        private bool CompareExistingToUpdatedOrganization(Organization org)
        {
            bool confirmUpdate = false;

            Organization updatedOrg = GetExistingOrganization(org.CustomerNumber, org.BranchNumber);

            if (org.IsKeithnetCustomer == updatedOrg.IsKeithnetCustomer && org.AchType == updatedOrg.AchType)
            {
                confirmUpdate = true;
            }
            else
            {
                eventLog.WriteErrorLog(String.Format("ETL:  Profile did not update correctly for customer {0} - branch {1}", org.CustomerNumber, org.BranchNumber));
            }

            return confirmUpdate;
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

        private enum ProfileOperationType
        {
            Create
            , Update
        }

        private enum ProfileObjectType
        {
            Organization
            , Address
        }

        public static string ToSQLFormat(string valueToParse)
        {
            //replace single quotes with triple quotes for insert
            valueToParse = valueToParse.Replace("'", "''");
            return valueToParse;
        }

        #endregion
    }
}

