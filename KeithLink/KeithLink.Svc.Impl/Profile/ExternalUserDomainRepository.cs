using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public class ExternalUserDomainRepository : Svc.Core.Interface.Profile.IUserDomainRepository
    {
        #region methods

        /// <summary>
        /// test user credentials
        /// </summary>
        /// <param name="userName">the person trying to login</param>
        /// <param name="password">the user's password</param>
        /// <returns>true if the credentials were correct</returns>
        /// <remarks>
        /// jwames - 8/1/2014 - original code
        /// jwames - 8/5/2014 - add tests for argument length
        /// </remarks>
        public bool AuthenticateUser(string userName, string password)
        {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (password.Length == 0) { throw new ArgumentException("password is required", "password"); }
            if (password == null) { throw new ArgumentNullException("password", "password is null"); }

            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain, Configuration.ActiveDirectoryExternalServerName))
                {
                    return principal.ValidateCredentials(userName, password, ContextOptions.SimpleBind);
                }
            }
            catch (Exception ex)
            {
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not authenticate user", ex);

                return false;
            }
        }

        public string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName)
        {
            const int NORMAL_ACCT = 0x200;
            const int PWD_NOTREQD = 0x20;

            string adPath = string.Format("LDAP://{0}:389/OU=users,OU={1},{2}", Configuration.ActiveDirectoryExternalServerName, customerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;
            // connect to the external AD server
            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.RefreshCache();
            } catch (Exception ex){
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            //string userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));
            string userName = GetNewUserName(emailAddress);

            DirectoryEntry newUser = null;
            // create the user
            try
            {
                string userCN = string.Format("CN={0}", userName);

                newUser = boundServer.Children.Add(userCN, "user");
                newUser.Properties["company"].Value = customerName;
                newUser.Properties["displayName"].Value = string.Format("{0} {1}", firstName, lastName);
                newUser.Properties["givenName"].Value = firstName;
                newUser.Properties["name"].Value = userName;
                newUser.Properties["sAmAccountName"].Value = userName;
                newUser.Properties["sn"].Value = lastName;
                newUser.Properties["userPrincipalName"].Value = emailAddress;
                newUser.Properties["userAccountControl"].Value = NORMAL_ACCT | PWD_NOTREQD;
                newUser.CommitChanges();
            }
            catch(Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not create user on external AD server.", ex);

                throw;
            }

            // set the user's password
            try
            {
                newUser.Invoke("SetPassword", new object[] { password });
                newUser.Properties["userAccountControl"].Value = NORMAL_ACCT;
                newUser.CommitChanges();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not set password for user on external AD server.", ex);

                throw;
            }

            // assign user to the specified role
            try
            {
                string groupName = string.Format("{0} {1}", customerName, roleName);
                string groupDN = string.Format("CN={0},OU=groups,OU={1},{2}", groupName, customerName, Configuration.ActiveDirectoryExternalRootNode);
                string groupPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, groupDN);

                DirectoryEntry group = new DirectoryEntry(groupPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                group.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                group.CommitChanges();
            }
            catch (Exception ex)
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl log = new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName);
                log.WriteErrorLog("Could not assign user to a role on external AD server.", ex);
                
                throw;
            }

            return userName;
        }
        
        /// <summary>
        /// return the domain and username
        /// </summary>
        /// <param name="userName">the user name without the domain name</param>
        /// <returns>string in the form of domain\username</returns>
        /// <remarks>
        /// jwames - 8/5/2014 - original code
        /// </remarks>
        private string GetDomainUserName(string userName)
        {
            return string.Format("{0}\\{1}", Configuration.ActiveDirectoryExternalDomain, userName);
        }
        
        public UserPrincipal GetUser(string userName)
        {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }

            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         GetDomainUserName(Configuration.ActiveDirectoryExternalUserName),
                                                                         Configuration.ActiveDirectoryExternalPassword))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);

                    return user;
                }
            }
            catch (Exception ex)
            {
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not get user", ex);

                return null;
            }
        }

        public string GetNewUserName(string emailAddress)
        {
            string userName = null;

            if (emailAddress.IndexOf('@') == -1)
                userName = emailAddress;
            else
                userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));

            if (UsernameExists(userName))
            {
                string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);
                DirectoryEntry boundServer = null;

                // connect to the external AD server
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

                DirectorySearcher userSearch = new DirectorySearcher(boundServer);
                userSearch.Filter = string.Format("cn={0}*", userName);

                SearchResultCollection results = userSearch.FindAll();

                // recursive call to make sure that the new user name does not also exist
                return GetNewUserName(string.Format("{0}{1}", userName, results.Count));
            }
            else
            {
                return userName;
            }
        }

        /// <summary>
        /// test to see if the user has access to the specified group
        /// </summary>
        /// <param name="userName">the user we are testing</param>
        /// <param name="groupName">the group</param>
        /// <returns>true if the user has access to the group</returns>
        /// <remarks>
        /// jwames - 8/4/2014 - original code
        /// jwames - 8/5/2014 - add argument validation
        /// </remarks>
        public bool IsInGroup(string userName, string groupName)
        {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (groupName.Length == 0) { throw new ArgumentException("groupName is required", "groupName"); }
            if (groupName == null) { throw new ArgumentNullException("groupName", "groupName is required"); }

            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         GetDomainUserName(Configuration.ActiveDirectoryExternalUserName),
                                                                         Configuration.ActiveDirectoryExternalPassword))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);
                    
                    if (user == null)
                        return false;
                    else
                        try
                        {
                            return user.IsMemberOf(principal, IdentityType.SamAccountName, groupName);
                        }
                        catch
                        {
                            return false;
                        }
                }
            }
            catch (Exception ex)
            {
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not get lookup users's role membership", ex);

                return false;
            }
        }

        public bool UsernameExists(string userName)
        {
            string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;
            // connect to the external AD server
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
                DirectorySearcher userSearch = new DirectorySearcher(boundServer);
                userSearch.Filter = string.Format("cn={0}", userName);

                SearchResultCollection results = userSearch.FindAll();

                if(results.Count >= 1)
                    return true;
                else
                    return false;
            } catch{
                return false;
            }

        }
        #endregion
    }
}
