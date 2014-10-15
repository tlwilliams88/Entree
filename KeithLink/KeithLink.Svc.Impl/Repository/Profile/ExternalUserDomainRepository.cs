using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Extensions;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class ExternalUserDomainRepository : ICustomerDomainRepository
    {
        #region attributes
        private enum UserAccountControlFlag
        {
            Disabled = 0x0002,
            Locked = 0x10,
            NormalAccount = 0x200,
            PasswordNotRequired = 0x20
        }

        IEventLogRepository _logger;
        ICustomerContainerRepository _containerRepo;
        #endregion

        #region ctor
        public ExternalUserDomainRepository(IEventLogRepository logger, ICustomerContainerRepository customerContainerRepo)
        {
            _logger = logger;
            _containerRepo = customerContainerRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// authenticates the user to the benekeith.com domain and throws an exception for any problems encountered
        /// </summary>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - change to call new authenticateuser method with a string output
        /// </remarks>
        public bool AuthenticateUser(string userName, string password)
        {
            string msg = null;
            bool success = AuthenticateUser(userName, password, out msg);

            if (success)
            {
                return true;
            }
            else
            {
                throw new ApplicationException(msg);
                //return false;
            }
        }

        /// <summary>
        /// test user credentials
        /// </summary>
        /// <param name="userName">the person trying to login</param>
        /// <param name="password">the user's password</param>
        /// <returns>true if the credentials were correct</returns>
        /// <remarks>
        /// jwames - 8/1/2014 - original code
        /// jwames - 8/5/2014 - add tests for argument length
        /// jwames - 8/15/2014 - add locking tests
        /// </remarks>
        public bool AuthenticateUser(string userName, string password, out string errorMessage)
        {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (password.Length == 0) { throw new ArgumentException("password is required", "password"); }
            if (password == null) { throw new ArgumentNullException("password", "password is null"); }

            errorMessage = null;

            // connect to server
            try
            {
                using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                            Configuration.ActiveDirectoryExternalServerName,
                                                            Configuration.ActiveDirectoryExternalRootNode,
                                                            ContextOptions.SimpleBind,
                                                            Configuration.ActiveDirectoryExternalDomainUserName,
                                                            Configuration.ActiveDirectoryExternalPassword))
                {
                    // if user exists
                    UserPrincipal authenticatingUser = UserPrincipal.FindByIdentity(boundServer, userName);

                    if (authenticatingUser == null)
                    {
                        errorMessage = "User name or password is invalid";
                        return false;
                    }

                    // if account is enabled 
                    if (authenticatingUser.Enabled == false)
                    {
                        errorMessage = "User account is disabled";
                        return false;
                    }

                    // if locked 
                    if (authenticatingUser.IsAccountLockedOut())
                    {
                        if (authenticatingUser.AccountLockoutTime.HasValue)
                        {
                            DateTime endOfLockout = authenticatingUser.AccountLockoutTime.Value.AddMinutes(Configuration.ActiveDirectoryLockoutDuration);

                            if (DateTime.Now < endOfLockout)
                            {
                                errorMessage = "User account is locked and cannot sign in now";
                                return false;
                            }
                        }

                    }

                    // validate password
                    if (boundServer.ValidateCredentials(userName, password, ContextOptions.SimpleBind))
                    {
                        return true;
                    }
                    else
                    {
                        if (authenticatingUser.BadLogonCount >= Configuration.ActiveDirectoryInvalidAttempts)
                        {
                            errorMessage = "User account is locked and cannot sign in now";
                        }
                        else
                        {
                            errorMessage = "User name or password is invalid";
                        }

                        return false;
                    }


                }
            }
            catch
            {
                errorMessage = "Could not connect to authentication server for benekeith.com";
                return false;
            }
        }

        /// <summary>
        /// create a user in the benekeith.com domain
        /// </summary>
        /// <remarks>
        /// jwames - 10/9/2014 - converted to use AccountManagement from original code
        /// </remarks>
        public string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName) {
            CustomerContainerReturn custExists = _containerRepo.SearchCustomerContainers(customerName);

            // create the customer container if it does not exist
            if (custExists.CustomerContainers.Count != 1) {
                _containerRepo.CreateCustomerContainer(customerName);
                _logger.WriteInformationLog(string.Format("New customer container created in Active Directory({0}).", customerName));
            }

            PrincipalContext principal = null;
            string userOU = string.Format("ou=Users,ou={0},{1}", customerName, Configuration.ActiveDirectoryExternalRootNode);

            try {
                principal = new PrincipalContext(ContextType.Domain,
                                                 Configuration.ActiveDirectoryExternalServerName,
                                                 userOU,
                                                 ContextOptions.Negotiate,
                                                 Configuration.ActiveDirectoryExternalDomainUserName,
                                                 Configuration.ActiveDirectoryExternalPassword);
            } catch(Exception ex){
                _logger.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            UserPrincipal user = new UserPrincipal(principal);

            user.DisplayName = string.Format("{0} {1}", firstName, lastName);
            user.GivenName = firstName;
            user.Surname = lastName;

            string userName = GetNewUserName(emailAddress);

            user.Name = userName;
            user.SamAccountName = userName;
            user.UserPrincipalName = emailAddress;

            user.SetPassword(password);
            user.Enabled = true;

            try {
                user.Save();
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not create user on external AD server.", ex);

                throw;
            }

            try {
                DirectoryEntry de = (DirectoryEntry)user.GetUnderlyingObject();
                de.Properties["company"].Value = customerName;

                de.CommitChanges();
            } catch (Exception ex) {
                _logger.WriteErrorLog("After user was created, could not set company name.", ex);

                throw;
            }

            JoinGroup(customerName, roleName, user);

            principal.Dispose();

            _logger.WriteInformationLog(string.Format("New user({0}) was created within the container ({1}) in Active Directory.", userName, customerName));

            return userName;
        }

        /// <summary>
        /// create a user in the benekeith.com domain
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public string CreateUser_Org(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName)
        {
            CustomerContainerReturn custExists = _containerRepo.SearchCustomerContainers(customerName);

            // create the customer container if it does not exist
            if (custExists.CustomerContainers.Count != 1) { 
                _containerRepo.CreateCustomerContainer(customerName);
                _logger.WriteInformationLog(string.Format("New customer container created in Active Directory({0}).", customerName));
            }

            const int NORMAL_ACCT = 0x200;
            const int PWD_NOTREQD = 0x20;

            string adPath = string.Format("LDAP://{0}:636/OU=users,OU={1},{2}", Configuration.ActiveDirectoryExternalServerName, customerName, Configuration.ActiveDirectoryExternalRootNode);

            DirectoryEntry boundServer = null;
            // connect to the external AD server
            try
            {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
                boundServer.RefreshCache();
            } catch (Exception ex){
                _logger.WriteErrorLog("Could not bind to external AD server.", ex);

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
                _logger.WriteErrorLog("Could not create user on external AD server.", ex);

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
                _logger.WriteErrorLog("Could not set password for user on external AD server.", ex);

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
                _logger.WriteErrorLog("Could not assign user to a role on external AD server.", ex);
                
                throw;
            }

            _logger.WriteInformationLog(string.Format("New user({0}) was created within the container ({1}) in Active Directory.", userName, customerName));

            return userName;
        }

        /// <summary>
        /// get a unique user name for AD based on the user name in the email address
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <returns>a user name</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public string GetNewUserName(string emailAddress) {
            string userName = null;

            if (emailAddress.IndexOf('@') == -1)
                userName = emailAddress;
            else
                userName = emailAddress.Substring(0, emailAddress.IndexOf('@'));

            if (UsernameExists(userName)) {
                string adPath = string.Format("LDAP://{0}:389/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);
                DirectoryEntry boundServer = null;

                // connect to the external AD server
                try {
                    boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                    boundServer.RefreshCache();
                } catch (Exception ex) {
                    _logger.WriteErrorLog("Could not bind to external AD server.", ex);

                    throw;
                }

                DirectorySearcher userSearch = new DirectorySearcher(boundServer);
                userSearch.Filter = string.Format("cn={0}*", userName);

                SearchResultCollection results = userSearch.FindAll();

                // recursive call to make sure that the new user name does not also exist
                return GetNewUserName(string.Format("{0}{1}", userName, results.Count));
            } else {
                return userName;
            }
        }

        /// <summary>
        /// get the user principal from the benekeith.com domain
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserPrincipal GetUser(string userName)
        {
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }

            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);
                    
                    return user;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteErrorLog("Could not get user", ex);

                return null;
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
        public bool IsInGroup(string userName, string groupName) {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (groupName.Length == 0) { throw new ArgumentException("groupName is required", "groupName"); }
            if (groupName == null) { throw new ArgumentNullException("groupName", "groupName is required"); }

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);
                    
                    if (user == null)
                        return false;
                    else
                        try {
                            return user.IsMemberOf(principal, IdentityType.SamAccountName, string.Format("{0} {1}", user.GetCompany(), groupName));
                        } catch {
                            return false;
                        }
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not get lookup users's role membership", ex);

                return false;
            }
        }

        public void JoinGroup(string customerName, string roleName, UserPrincipal user) {
            string groupOU = string.Format("ou=Groups,ou={0},{1}", customerName, Configuration.ActiveDirectoryExternalRootNode);

            using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                 Configuration.ActiveDirectoryExternalServerName,
                                                 groupOU,
                                                 ContextOptions.Negotiate,
                                                 Configuration.ActiveDirectoryExternalDomainUserName,
                                                 Configuration.ActiveDirectoryExternalPassword)) {
                string groupName = string.Join(" ", new string[] { customerName, roleName });

                GroupPrincipal group = GroupPrincipal.FindByIdentity(principal, groupName);

                // have to use directory entries because the computer making the call is not necessarily on the domain
                DirectoryEntry de = (DirectoryEntry)group.GetUnderlyingObject();

                de.Properties["member"].Add(user.DistinguishedName);

                try {
                    de.CommitChanges();
                } catch (Exception ex) {
                    _logger.WriteErrorLog(string.Format("Could not add user ({0}) to group ({1}).", user.Name, groupName), ex);

                    throw;
                }    
            }
        }

        /// <summary>
        /// change the password for the user
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="oldPassword">the original password</param>
        /// <param name="newPassword">the new password</param>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public bool UpdatePassword(string emailAddress, string oldPassword, string newPassword) {
            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, emailAddress);

                    if (user == null) {
                        throw new ApplicationException("Email address not found");
                    }

                    if (principal.ValidateCredentials(emailAddress, oldPassword)) {
                        user.ChangePassword(oldPassword, newPassword);
                        
                        return true;
                    } else {
                        if (user.IsAccountLockedOut()) {
                            throw new ApplicationException("User account is locked and cannot sign in now");
                        } else { 
                            throw new ApplicationException("Invalid password");
                        }
                    }

                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not change user's password", ex);

                throw;
            }
        }

        /// <summary>
        /// the original update password method that is still used to aid in network testing
        /// </summary>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public bool UpdatePassword_Org(string emailAddress, string oldPassword, string newPassword) {
            string adPath = string.Format("LDAP://{0}:636/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            string loginErrMsg = null;
            if (AuthenticateUser(emailAddress, oldPassword, out loginErrMsg) == false) {
                if (string.Compare(loginErrMsg, "user name or password is invalid", true) == 0) {
                    return false;
                } else {
                    throw new ApplicationException(loginErrMsg);
                }
            }

            DirectoryEntry boundServer = null;
            // connect to the external AD server
            try {
                boundServer = new DirectoryEntry(adPath, Configuration.ActiveDirectoryExternalUserName, Configuration.ActiveDirectoryExternalPassword);
                boundServer.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
                boundServer.RefreshCache();
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not bind to external AD server.", ex);

                throw;
            }

            DirectoryEntry currentUser = null;

            try {
                DirectorySearcher adSearch = new DirectorySearcher(boundServer, string.Concat("userPrincipalName=", emailAddress));
                currentUser = adSearch.FindOne().GetDirectoryEntry();
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not find user on external Ad server.", ex);

                throw;
            }

            // set the user's password
            try {
                //currentUser.AuthenticationType = AuthenticationTypes.Secure;
                currentUser.Invoke("SetPassword", new object[] { newPassword });
                currentUser.CommitChanges();
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not change password for user on external AD server.", ex);

                throw;
            }

            return true;
        }

        /// <summary>
        /// update attributes for a user
        /// </summary>
        /// <param name="oldEmailAddress">the original address</param>
        /// <param name="newEmailAdress">if the email address is changing, it is the new password. If it is not changing it can be blank or the old password</param>
        /// <param name="firstName">the updated first name</param>
        /// <param name="lastName">the updated last name</param>
        /// <remarks>
        /// jwames - 10/3/2014 - documented
        /// </remarks>
        public void UpdateUserAttributes(string oldEmailAddress, string newEmailAdress, string firstName, string lastName) {
            if (oldEmailAddress == null) { throw new ArgumentNullException("oldEmailAddress", "oldEmailAddress is null"); }
            if (oldEmailAddress.Length == 0) { throw new ArgumentException("oldEmailAddress is required", "oldEmailAddress"); }

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, oldEmailAddress);

                    if (newEmailAdress.Length > 0 && string.Compare(oldEmailAddress, newEmailAdress, true) != 0) {
                        user.UserPrincipalName = newEmailAdress;
                    }

                    user.GivenName = firstName;
                    user.Surname = lastName;
                    user.DisplayName = string.Format("{0} {1}", firstName, lastName);

                    user.Save();
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not update user", ex);
                throw;
            }
        }

        /// <summary>
        /// check the benekeith.com domain for the username
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
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
                _logger.WriteErrorLog("Could not bind to external AD server.", ex);

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
