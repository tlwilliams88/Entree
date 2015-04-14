using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Linq;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Models.Authentication;
using KeithLink.Svc.Core.Enumerations.Authentication;
using System.Text.RegularExpressions;

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

        private enum PassworedExpiredFlag {
            Disabled = -1,
            Enabled = 0
        }

        IEventLogRepository _logger;
        //ICustomerContainerRepository _containerRepo;
        #endregion

        #region ctor
        public ExternalUserDomainRepository(IEventLogRepository logger)
        {
            _logger = logger;
            //_containerRepo = customerContainerRepo;
        }
        #endregion

        #region methods
        private void AddUserToGroup(UserPrincipal user, string groupName, DirectoryEntry group) {
            group.Properties["member"].Add(user.DistinguishedName);

            try {
                group.CommitChanges();
            } catch (Exception ex) {
                _logger.WriteErrorLog(string.Format("Could not add user ({0}) to group ({1}).", user.Name, groupName), ex);
                throw;
            }
        }

        private void AddUserToOu(UserPrincipal user, string groupName, DirectoryEntry ou) {
            //DirectoryEntry userInGroup = ou.Container.Add(user.GetUnderlyingObject());

            try {
                ou.CommitChanges();
            } catch (Exception ex) {
                _logger.WriteErrorLog(string.Format("Could not add user ({0}) to group ({1}).", user.Name, groupName), ex);
                throw;
            }
        }

        /// <summary>
        /// authenticates the user to the benekeith.com domain and throws an exception for any problems encountered
        /// </summary>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - change to call new authenticateuser method with a string output
        /// </remarks>
        //public bool AuthenticateUser(string userName, string password)
        //{
        //    AuthenticationModel success = AuthenticateUser(userName, password);

        //    if (AuthenticationStatus.)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        throw new ApplicationException(msg);
        //        //return false;
        //    }
        //}

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
        public AuthenticationModel AuthenticateUser(string userName, string password)
        {
            AuthenticationModel returnValue = new AuthenticationModel(); 

            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (password.Length == 0) { throw new ArgumentException("password is required", "password"); }
            if (password == null) { throw new ArgumentNullException("password", "password is null"); }

            returnValue.Message = null;

            // connect to server
            try
            {
                using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                            Configuration.ActiveDirectoryExternalServerName,
                                                            Configuration.ActiveDirectoryExternalRootNode,
                                                            ContextOptions.Negotiate,
                                                            Configuration.ActiveDirectoryExternalDomainUserName,
                                                            Configuration.ActiveDirectoryExternalPassword))
                {
                    // if user exists
                    UserPrincipal authenticatingUser = UserPrincipal.FindByIdentity(boundServer, userName);

                    if (authenticatingUser == null)
                    {
                        returnValue.Message = "User name or password is invalid";
                        returnValue.Status = AuthenticationStatus.FailedAuthentication;
                        return returnValue;
                    }

                    // if account is enabled 
                    if (authenticatingUser.Enabled == false)
                    {
                        returnValue.Message = "User account is disabled";
                        returnValue.Status = AuthenticationStatus.Disabled;
                        return returnValue;
                    }

                    // if locked 
                    if (authenticatingUser.IsAccountLockedOut())
                    {
                        if (authenticatingUser.AccountLockoutTime.HasValue)
                        {
                            DateTime endOfLockout = authenticatingUser.AccountLockoutTime.Value.AddMinutes(Configuration.ActiveDirectoryLockoutDuration);

                            if (DateTime.Now < endOfLockout)
                            {
                                returnValue.Message = "User account is locked and cannot sign in now";
                                returnValue.Status = AuthenticationStatus.Locked;
                                return returnValue;
                            }
                        }

                    }

                    // if password is expired
                    if (authenticatingUser.LastPasswordSet == null ) {
                        returnValue.Status = AuthenticationStatus.PasswordExpired;
                        // Have to turn off the expired password flag to validate the credentials. It is turned back on after authentication
                        SetExpiredPassword( (DirectoryEntry)authenticatingUser.GetUnderlyingObject(), PassworedExpiredFlag.Disabled );
                    }
                    
                    // validate password
                   if (boundServer.ValidateCredentials(userName, password, ContextOptions.SimpleBind))
                    {
                        if (returnValue.Status.Equals( AuthenticationStatus.PasswordExpired )) {
                            // Turn the password expired flag back on
                            SetExpiredPassword( (DirectoryEntry)authenticatingUser.GetUnderlyingObject(), PassworedExpiredFlag.Enabled );
                            returnValue.Message = "Password is expired";
                            return returnValue;
                        } else {
                            returnValue.Status = AuthenticationStatus.Successful;
                            return returnValue;
                        }
                    }
                    else
                    {
                        // If we disabled the expired password we need to re-enable it - we did not fail authentication due to an expired password here so we let the status get changed back.
                        if (returnValue.Status.Equals( AuthenticationStatus.PasswordExpired )) {
                            SetExpiredPassword( (DirectoryEntry)authenticatingUser.GetUnderlyingObject(), PassworedExpiredFlag.Enabled );
                        }

                        if (authenticatingUser.BadLogonCount >= Configuration.ActiveDirectoryInvalidAttempts)
                        {
                            returnValue.Message = "User account is locked and cannot sign in now";
                            returnValue.Status = AuthenticationStatus.Locked;
                        }
                        else
                        {
                            returnValue.Message = "User name or password is invalid";
                            returnValue.Status = AuthenticationStatus.FailedAuthentication;
                        }



                        return returnValue;
                    }
                }
            }
            catch
            {
                returnValue.Message = "Coult not connect to authentication server for benekeith.com";
                returnValue.Status = AuthenticationStatus.FailedConnectingToAuthenticationServer;
                return returnValue;
            }
        }

        //private string CleanseOfInvalidChars(string input) {
        //    return Regex.Replace(input, @"[""#&\[\]:\<\>+=\*/;,?@\\]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
        //}

        //private DirectoryEntry CreateChildOrganizationalUnit(string customerName, DirectoryEntry parent) {
        //    DirectoryEntry newOU = parent.Children.Add("OU=" + CleanseOfInvalidChars(customerName), "OrganizationalUnit");
        //    newOU.CommitChanges();
        //    parent.RefreshCache();
        //    return newOU;
        //}

        //private void CreateCustomerSecurityGroup(string customerName, string roleName, DirectoryEntry customerContainer, DirectoryEntry groupContainer) {
        //    DirectoryEntry newGroup = groupContainer.Children.Add("CN=" + CleanseOfInvalidChars(customerName) + " " + roleName, "group");
        //    newGroup.CommitChanges();
        //    customerContainer.RefreshCache();
        //}

        /// <summary>
        /// create a user in the benekeith.com domain
        /// </summary>
        /// <remarks>
        /// jwames - 10/9/2014 - converted to use AccountManagement from original code
        /// jwames - 4/1/2015 - remove the customer container support
        /// jwames - 4/14/2014 - change the userOU to use the container from the config file
        /// </remarks>
        public string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName) {
            //CustomerContainerReturn custExists = _containerRepo.SearchCustomerContainers(customerName);

            // create the customer container if it does not exist
            //if (custExists.CustomerContainers.Count != 1) {
            //    _containerRepo.CreateCustomerContainer(customerName);
            //    _logger.WriteInformationLog(string.Format("New customer container created in Active Directory({0}).", customerName));
            //}

            PrincipalContext principal = null;
            //string userOU = string.Format("ou=Users,ou={0},{1}", customerName, Configuration.ActiveDirectoryExternalRootNode);
            //string userOU = string.Format("ou=Users,{0}", Configuration.ActiveDirectoryExternalRootNode);
            string userOU = string.Format("ou={0},{1}", Configuration.ActiveDirectoryExternalUserContainer, Configuration.ActiveDirectoryExternalRootNode);

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
            //CustomerContainerReturn custExists = _containerRepo.SearchCustomerContainers(customerName);

            //// create the customer container if it does not exist
            //if (custExists.CustomerContainers.Count != 1) { 
            //    _containerRepo.CreateCustomerContainer(customerName);
            //    _logger.WriteInformationLog(string.Format("New customer container created in Active Directory({0}).", customerName));
            //}

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

        public void ExpirePassword(string emailAddress) {
            using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                            Configuration.ActiveDirectoryExternalServerName,
                                                            Configuration.ActiveDirectoryExternalRootNode,
                                                            ContextOptions.Negotiate,
                                                            Configuration.ActiveDirectoryExternalDomainUserName,
                                                            Configuration.ActiveDirectoryExternalPassword)) {
                // if user exists
                UserPrincipal user = UserPrincipal.FindByIdentity(boundServer, emailAddress);

                SetExpiredPassword((DirectoryEntry)user.GetUnderlyingObject(), PassworedExpiredFlag.Enabled);
            }
        }

        //private DirectoryEntry FindChildOu(DirectoryEntry parent, string childOuName) {
        //    try {
        //        return parent.Children.Find("OU=" + CleanseOfInvalidChars(childOuName));
        //    } catch {
        //        return null;
        //    }
        //}

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

        private GroupPrincipal GetSecurityGroup(PrincipalContext principal, string groupName) {
            GroupPrincipal group = null;
            //Regex pattern = new Regex(,\t\r ]|[\n]{2}");
            //pattern.Replace(myString, "\n");
            try {
                //group = GroupPrincipal.FindByIdentity(principal, CleanseOfInvalidChars(groupName));
                group = GroupPrincipal.FindByIdentity(principal, groupName);
            } catch (Exception e) {
                _logger.WriteInformationLog("Unabe to read security group: " + groupName, e);
            }
            return group;
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
        public string GetUserGroup(string userName, List<string> groupNames) {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (groupNames.Count == 0) { throw new ArgumentException("groupName is required", "groupName"); }
            if (string.IsNullOrEmpty(groupNames.FirstOrDefault())) { throw new ArgumentNullException("groupName", "groupName is required"); }

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
                        return string.Empty;
                    else
                    {
                        try
                        {
                            foreach (string g in ((DirectoryEntry)user.GetUnderlyingObject()).Properties["memberOf"])
                            {
                                //string groupName = g.Substring(3, (g.ToLower().IndexOf(",ou=") - 3)).ToLower(); // format is CN=company name role,OU=groups...
                                //string groupName = g.Substring(3, (g.ToLower().IndexOf(",ou=") - 3)).ToLower(); // format is CN=company name role,OU=groups...
                                foreach (string s in groupNames)
                                {
                                    if (g.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                        return s;
                                    }
                                    //if (groupName.EndsWith(s.ToLower()))
                                    //    return s;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteErrorLog("Error loading group", ex);
                        }
                    }
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not get lookup users's role membership", ex);
            }
            return "guest";
        }

        public void GrantAccess(string userName, string roleName) {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (roleName.Length == 0) { throw new ArgumentException("roleName is required", "roleName"); }
            if (roleName == null) { throw new ArgumentNullException("roleName", "roleName is null"); }

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(principal, IdentityType.Name, roleName);

                    if (userName == null || group == null) {
                        return;
                    } else {
                        DirectoryEntry groupDE = (DirectoryEntry) group.GetUnderlyingObject();

                        groupDE.Properties["member"].Add(user.DistinguishedName);

                        groupDE.CommitChanges();
                    }
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog(string.Format("Could not add user ({0}) to group ({1}).", userName, roleName), ex);
            }
        }

        public bool HasAccess(string userName, string roleName) {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (roleName.Length == 0) { throw new ArgumentException("roleName is required", "roleName"); }
            if (roleName == null) { throw new ArgumentNullException("roleName", "roleName is null"); }

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);

                    if (user == null) { return false; }

                    bool groupFound = false;

                    foreach (string g in ((DirectoryEntry)user.GetUnderlyingObject()).Properties["memberOf"]) {
                        string groupName = g.Substring(3, (g.ToLower().IndexOf(",ou=") - 3)); // format is CN=company name role,OU=groups...

                        if (groupName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)) {
                            groupFound = true;
                            break;
                        }
                    }

                    return groupFound;   
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not get lookup users's role membership", ex);
                return false;
            }
        }

        public bool IsPasswordExpired(string emailAddress) {
            using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                            Configuration.ActiveDirectoryExternalServerName,
                                                            Configuration.ActiveDirectoryExternalRootNode,
                                                            ContextOptions.Negotiate,
                                                            Configuration.ActiveDirectoryExternalDomainUserName,
                                                            Configuration.ActiveDirectoryExternalPassword)) {
                // if user exists
                UserPrincipal user = UserPrincipal.FindByIdentity(boundServer, emailAddress);

                return (user.LastPasswordSet.Equals(null)) ? true : false;
            }
        }

        public void JoinGroup(string customerName, string roleName, UserPrincipal user) {
            //string groupOU = string.Format("ou=Groups,ou={0},{1}", CleanseOfInvalidChars(customerName), Configuration.ActiveDirectoryExternalRootNode);
            string groupOU = string.Format("ou=Groups,{0}", Configuration.ActiveDirectoryExternalRootNode);
            string adPath = string.Format("LDAP://{0}:636/{1}", Configuration.ActiveDirectoryExternalServerName, Configuration.ActiveDirectoryExternalRootNode);

            using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                 Configuration.ActiveDirectoryExternalServerName,
                                                 groupOU,
                                                 ContextOptions.Negotiate,
                                                 Configuration.ActiveDirectoryExternalDomainUserName,
                                                 Configuration.ActiveDirectoryExternalPassword)) {
                //string groupName = string.Join(" ", new string[] { customerName, roleName });

				//groupName = CleanseOfInvalidChars(groupName);

                //GroupPrincipal group = GetSecurityGroup(principal, groupName);
                GroupPrincipal group = GetSecurityGroup(principal, roleName);

                //// check for root, create if absent; have to use directory entries because the computer making the call is not necessarily on the domain
                //DirectoryEntry root = new DirectoryEntry(
                //    adPath,
                //    Configuration.ActiveDirectoryExternalDomainUserName,
                //    Configuration.ActiveDirectoryExternalPassword);
                //DirectoryEntry customerContainer = FindChildOu(root, customerName);

                //if (customerContainer == null)
                //    customerContainer = CreateChildOrganizationalUnit(customerName, root);
                //DirectoryEntry groupContainer = FindChildOu(customerContainer, "groups");
                //if (groupContainer == null)
                //    groupContainer = CreateChildOrganizationalUnit("groups", customerContainer);
                //DirectoryEntry userContainer = FindChildOu(customerContainer, "users");
                //if (userContainer == null)
                //    userContainer = CreateChildOrganizationalUnit("users", customerContainer);

                //// didn't find group at the beginning, so must need to create it
                //if (group == null)
                //{
                //    CreateCustomerSecurityGroup(customerName, roleName, customerContainer, groupContainer);

                //    group = GroupPrincipal.FindByIdentity(principal, groupName);
                //}
                //AddUserToGroup(user, groupName, (DirectoryEntry)group.GetUnderlyingObject());
                AddUserToGroup(user, roleName, (DirectoryEntry)group.GetUnderlyingObject());
                //AddUserToOu(user, "users", userContainer);
            }
        }

        public void RevokeAccess(string userName, string roleName) {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }
            if (roleName.Length == 0) { throw new ArgumentException("roleName is required", "roleName"); }
            if (roleName == null) { throw new ArgumentNullException("roleName", "roleName is null"); }

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, userName);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(principal, IdentityType.Name, roleName);

                    if (userName == null || group == null) {
                        return;
                    } else {
                        DirectoryEntry groupDE = (DirectoryEntry)group.GetUnderlyingObject();

                        groupDE.Properties["member"].Remove(user.DistinguishedName);

                        groupDE.CommitChanges();
                    }
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog(string.Format("Could not add user ({0}) to group ({1}).", userName, roleName), ex);
            }
        }

        private void SetExpiredPassword(DirectoryEntry entry, PassworedExpiredFlag status) {
            entry.Properties["pwdLastSet"].Value = status;
            entry.CommitChanges();
        }

        public void UnlockAccount(string emailAddress) {
            using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                Configuration.ActiveDirectoryExternalServerName,
                                                Configuration.ActiveDirectoryExternalRootNode,
                                                ContextOptions.Negotiate,
                                                Configuration.ActiveDirectoryExternalDomainUserName,
                                                Configuration.ActiveDirectoryExternalPassword)) {
                UserPrincipal user = UserPrincipal.FindByIdentity(boundServer, IdentityType.UserPrincipalName, emailAddress);

                user.UnlockAccount();
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
        /// change the password for the user without previous password
        /// </summary>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="newPassword">the new password</param>
        /// <returns>true if successful</returns>
        /// <remarks>
        /// mdjoiner - 02/05/2015 - documented
        /// </remarks>
        public void UpdatePassword(string emailAddress, string newPassword) {
            try {
                using (PrincipalContext principal = new PrincipalContext( ContextType.Domain,
                                                                         Configuration.ActiveDirectoryExternalServerName,
                                                                         Configuration.ActiveDirectoryExternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryExternalDomainUserName,
                                                                         Configuration.ActiveDirectoryExternalPassword )) {
                    UserPrincipal user = UserPrincipal.FindByIdentity( principal, emailAddress );

                    if (user == null) {
                        throw new ApplicationException( "Email address not found" );
                    }

                    user.SetPassword( newPassword );
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

            AuthenticationModel authentication = AuthenticateUser( emailAddress, oldPassword );
            if (authentication.Status.Equals(AuthenticationStatus.Successful) == false) {
                if (authentication.Status.Equals(AuthenticationStatus.FailedAuthentication)) {
                    return false;
                } else {
                    throw new ApplicationException(authentication.Message);
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

        //public void UpdateUserGroups(List<string> customerNames, string roleName, string userEmail) {
        //    // clear & recreate roles?  or, call isingroup and only add if necessary?  not sure...  what to do about account users?
        //    // talked to jeremy, guess we are stuck doing per-customer role stuff...  
        //    // will be a little messy when updating accounts or adding customers to accounts; at one point, Jeremy said we could
        //    // just put them in the first customer, but since changed his story and we will be double record keeping, so need to synch
        //    // CS and AD... :(  Could be slow for users with a lot of customers assigned; hopefully that isn't the common case.
        //    // may fall back to what Jeremy said and just put a user in the first OU....
        //    // 

        //    // 1st, check all groups user is in, and clear all but authenticated domain users...
        //    using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
        //                                                                Configuration.ActiveDirectoryExternalServerName,
        //                                                                Configuration.ActiveDirectoryExternalRootNode,
        //                                                                ContextOptions.Negotiate,
        //                                                                Configuration.ActiveDirectoryExternalDomainUserName,
        //                                                                Configuration.ActiveDirectoryExternalPassword)) {
        //        UserPrincipal user = UserPrincipal.FindByIdentity(principal, userEmail);

        //        if (user == null)
        //            throw new ApplicationException("Unable to read user for role assignment");
        //        else {
        //            // remove user from all but authenticated user roles
        //            DirectoryEntry de = (DirectoryEntry)user.GetUnderlyingObject();
        //            object groups = de.Invoke("Groups");
        //            string tmp = string.Empty;

        //            List<DirectoryEntry> groupsUserIsMemberOf = new List<DirectoryEntry>();
        //            foreach (object g in (IEnumerable)groups)
        //                groupsUserIsMemberOf.Add(new DirectoryEntry(g));

        //            groupsUserIsMemberOf.Select(g => tmp += "\r\n" + g);
        //            foreach (DirectoryEntry g in groupsUserIsMemberOf) {
        //                if (g.Name.EndsWith(Configuration.AccessGroupKbitCustomer, StringComparison.InvariantCultureIgnoreCase) ||
        //                    g.Name.EndsWith(Configuration.AccessGroupKbitAdmin, StringComparison.InvariantCultureIgnoreCase)) {
        //                    // do not mess with access groups
        //                } else {
        //                    g.Properties["member"].Remove(user.DistinguishedName);
        //                    g.CommitChanges();
        //                }
        //            }
        //            // then add them back to all of the new customer by role, then done
        //            if (customerNames.Count > 0) {
        //                try {
        //                    de.Properties["company"].Value = customerNames.FirstOrDefault();
        //                    de.CommitChanges();
        //                } catch (Exception ex) {
        //                    _logger.WriteErrorLog("Updating Roles, Could not set company name.", ex);
        //                    throw;
        //                }
        //            }
        //            foreach (string customerName in customerNames) {

        //                JoinGroup(customerName, roleName, user);

        //            }
        //        }
        //    }
        //}

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
