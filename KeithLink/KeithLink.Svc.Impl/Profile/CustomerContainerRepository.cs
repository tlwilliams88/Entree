using KeithLink.Svc.Core.Models.Profile;
using System;
using System.DirectoryServices;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class CustomerContainerRepository : KeithLink.Svc.Core.Interface.Profile.ICustomerContainerRepository
    {
        #region methods

        /// <summary>
        /// create a customer container in active directory with a user and group OU with related groups
        /// </summary>
        /// <param name="customerName">the name of the customer container to create</param>
        /// <remarks>
        /// jwames - 8/6/2014 - original code
        /// </remarks>
        public void CreateCustomerContainer(string customerName)
        {
            if (customerName.Length == 0) { throw new ArgumentException("customerName is required", "customerName"); }
            if (customerName == null) { throw new ArgumentNullException("customerName", "customerName is null"); }
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ArgumentException("customer name cannot contain special characters", "customerName"); }

            string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);
            string ouPath = string.Format("OU={0}", customerName);

            DirectoryEntry boundServer = null;

            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.RefreshCache();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            try
            {
                DirectoryEntry newOrgUnit = boundServer.Children.Add(ouPath, "OrganizationalUnit");
                newOrgUnit.Properties["description"].Add(string.Format("Customer Container for {0}", customerName));
                newOrgUnit.CommitChanges();

                DirectoryEntry userOrgUnit = newOrgUnit.Children.Add("OU=Users", "OrganizationalUnit");
                userOrgUnit.CommitChanges();

                DirectoryEntry groupOrgUnit = newOrgUnit.Children.Add("OU=Groups", "OrganizationalUnit");
                groupOrgUnit.CommitChanges();

                string acctGroupName = string.Format("{0} {1}", customerName, Core.Constants.ROLE_EXTERNAL_ACCOUNTING);
                DirectoryEntry acctGroup = groupOrgUnit.Children.Add(string.Format("CN={0}", acctGroupName), "group");
                acctGroup.Properties["sAmAccountName"].Value = acctGroupName;
                acctGroup.CommitChanges();

                string ownGroupName = string.Format("{0} {1}", customerName, Core.Constants.ROLE_EXTERNAL_OWNER);
                DirectoryEntry ownGroup = groupOrgUnit.Children.Add(string.Format("CN={0}", ownGroupName), "group");
                ownGroup.Properties["sAmAccountName"].Value = ownGroupName;
                ownGroup.CommitChanges();

                string purchGroupName = string.Format("{0} {1}", customerName, Core.Constants.ROLE_EXTERNAL_PURCHASING);
                DirectoryEntry purchGroup = groupOrgUnit.Children.Add(string.Format("CN={0}", purchGroupName), "group");
                purchGroup.Properties["sAmAccountName"].Value = purchGroupName;
                purchGroup.CommitChanges();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not create organizational unit on external AD server.", ex);

                throw;
            }
        }

        /// <summary>
        /// delete the customer container and everything within it
        /// </summary>
        /// <param name="customerName">the name of the customer container</param>
        /// <remarks>
        /// jwames - 8/6/2014 - original code
        /// </remarks>
        public void DeleteCustomerContainer(string customerName)
        {
            if (customerName.Length == 0) { throw new ArgumentException("customerName is required", "customerName"); }
            if (customerName == null) { throw new ArgumentNullException("customerName", "customerName is null"); }
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ArgumentException("customer name cannot contain special characters", "customerName"); }

            string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;

            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.RefreshCache();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            try
            {
                DirectorySearcher searcher = new DirectorySearcher(boundServer);
                searcher.Filter = string.Format("(OU={0})", customerName);

                DirectoryEntry entry = searcher.FindOne().GetDirectoryEntry();

                if (entry != null) { entry.DeleteTree(); }
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not delete organizational unit on external AD server.", ex);

                throw;
            }
        }

        /// <summary>
        /// return the specified container
        /// </summary>
        /// <param name="customerName">the name of the customer container</param>
        /// <returns>CustomerContainerReturn</returns>
        /// <remarks>
        /// jwames - 8/7/2014 - original code
        /// jwames - 8/11/2014 - remove the OU= from the string returned
        /// </remarks>
        public CustomerContainerReturn GetCustomerContainer(string customerName)
        {
            if (customerName.Length == 0) { throw new ArgumentException("customerName is required", "customerName"); }
            if (customerName == null) { throw new ArgumentNullException("customerName", "customerName is null"); }
            if (System.Text.RegularExpressions.Regex.IsMatch(customerName, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ArgumentException("customer name cannot contain special characters", "customerName"); }

            string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;

            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.RefreshCache();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            CustomerContainerReturn retVal = new CustomerContainerReturn();

            try
            {
                DirectorySearcher searcher = new DirectorySearcher(boundServer);
                searcher.Filter = string.Format("(OU={0})", customerName);

                DirectoryEntry entry = searcher.FindOne().GetDirectoryEntry();

                if (entry != null)
                {
                    // do not include the "OU=" in the name that is returned
                    retVal.CustomerContainers.Add(new CustomerContainer(entry.Name.Substring(3)));
                }
            }
            catch (NullReferenceException) { 
                // do nothing as this is expected when no match is found
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not find organizational unit on external AD server.", ex);

                throw;
            }

            return retVal;
        }

        /// <summary>
        /// returns a list of customer containers using a starts with query
        /// </summary>
        /// <param name="searchText">the text we looking for</param>
        /// <returns>CustomerContainerReturn</returns>
        /// <remarks>
        /// jwames - 8/7/2014 - original code
        /// jwames - 8/11/2014 - remove the OU= from the string returned
        /// </remarks>
        public CustomerContainerReturn SearchCustomerContainers(string searchText)
        {
            if (searchText.Length == 0) { throw new ArgumentException("searchText is required", "searchText"); }
            if (searchText == null) { throw new ArgumentNullException("searchText", "searchText is null"); }
            if (System.Text.RegularExpressions.Regex.IsMatch(searchText, Core.Constants.REGEX_AD_ILLEGALCHARACTERS)) { throw new ArgumentException("customer name cannot contain special characters", "searchText"); }

            string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;

            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.RefreshCache();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            CustomerContainerReturn retVal = new CustomerContainerReturn();

            try
            {
                DirectorySearcher searcher = new DirectorySearcher(boundServer);
                searcher.Filter = string.Format("(OU={0}*)", searchText);

                SearchResultCollection results = searcher.FindAll();

                if (results.Count > 0)
                {
                    foreach (SearchResult result in results)
                    {
                        // do not include "OU=" in the name that is returned
                        retVal.CustomerContainers.Add(new CustomerContainer(result.GetDirectoryEntry().Name.Substring(3)));
                    }
                }
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not find organizational units on external AD server.", ex);

                throw;
            }

            return retVal;
        }

        #endregion
    }
}
