using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Interface.Profile;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Text;
using System.Linq;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class InternalUserDomainRepository : IUserDomainRepository
    {
        #region attributes
        IEventLogRepository _logger;
        #endregion

        #region ctor
        public InternalUserDomainRepository(IEventLogRepository logger)
        {
            _logger = logger;
        }
        #endregion

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
        /// jwames - 8/18/2014 - change to throw exceptions when authentication fails
        /// </remarks>
        public bool AuthenticateUser(string emailAddress, string password) {
            string msg = null;
            bool success = AuthenticateUser(emailAddress, password, out msg);

            if (success) {
                return true;
            } else {
                throw new ApplicationException(msg);
                //return false;
            }
        }

        /// <summary>
        /// authenticate the user and return an error message when authentication fails
        /// </summary>
        /// <param name="userName">the user's network user id</param>
        /// <param name="password">the user's password</param>
        /// <param name="errorMessage">reason for failing authentication</param>
        /// <returns>true/false</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public bool AuthenticateUser(string emailAddress, string password, out string errorMessage)
        {
            if (emailAddress == null) { throw new ArgumentNullException("emailAddress", "emailAddress is null"); }
            if (emailAddress.Length == 0) { throw new ArgumentException("emailAddress is required", "emailAddress"); }
            if (password == null) { throw new ArgumentNullException("password", "password is null"); }
            if (password.Length == 0) { throw new ArgumentException("password is required", "password"); }

            errorMessage = null;

            string userName = emailAddress.Split('@')[0];

            // before authenticating, confirm that user is on whitelist
            if (Configuration.WhiteListedBekUsersEnforced)
            {
                if (!Configuration.WhiteListedBekUsers.Contains(userName.ToLower()))
                {
                    errorMessage = "Internal User Not Whitelisted";
                    return false;
                }
            }

            // connect to server
            try {
                using (PrincipalContext boundServer = new PrincipalContext(ContextType.Domain,
                                                            Configuration.ActiveDirectoryInternalServerName,
                                                            Configuration.ActiveDirectoryInternalRootNode,
                                                            ContextOptions.Negotiate,
                                                            Configuration.ActiveDirectoryInternalDomainUserName,
                                                            Configuration.ActiveDirectoryInternalPassword)) {
                    // if user exists
                    UserPrincipal authenticatingUser = UserPrincipal.FindByIdentity(boundServer, IdentityType.SamAccountName, userName);

                    if (authenticatingUser == null) {
                        errorMessage = "User name or password is invalid";
                        return false;
                    }

                    // if account is enabled 
                    if (authenticatingUser.Enabled == false) {
                        errorMessage = "User account is disabled";
                        return false;
                    }

                    // if locked 
                    if (authenticatingUser.IsAccountLockedOut()) {
                        if (authenticatingUser.AccountLockoutTime.HasValue) {
                            DateTime endOfLockout = authenticatingUser.AccountLockoutTime.Value.AddMinutes(Configuration.ActiveDirectoryLockoutDuration);

                            if (DateTime.Now < endOfLockout) {
                                errorMessage = "User account is locked and cannot sign in now";
                                return false;
                            }
                        }
                    }

                    // validate password
                    if (boundServer.ValidateCredentials(userName, password)) {
                        return true;
                    } else {
                        if (authenticatingUser.BadLogonCount >= Configuration.ActiveDirectoryInvalidAttempts) 
                            errorMessage = "User account is locked and cannot sign in now";
                        else
                            errorMessage = "User name or password is invalid";

                        return false;
                    }
                }
            } catch {
                errorMessage = "Could not connect to authentication server for benekeith.com";
                return false;
            }
        }

        /// <summary>
        /// get the user from AD
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserPrincipal GetUser(string emailAddress)
        {
            if (emailAddress.Length == 0) { throw new ArgumentException("emailAddress is required", "emailAddress"); }
            if (emailAddress == null) { throw new ArgumentNullException("emailAddress", "userName is null"); }

            string userName = emailAddress.Split('@')[0];

            try {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryInternalServerName,
                                                                         Configuration.ActiveDirectoryInternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryInternalDomainUserName,
                                                                         Configuration.ActiveDirectoryInternalPassword)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, IdentityType.SamAccountName, userName);

                    return user;
                }
            } catch (Exception ex) {
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
                                                                         Configuration.ActiveDirectoryInternalServerName,
                                                                         Configuration.ActiveDirectoryInternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         Configuration.ActiveDirectoryInternalDomainUserName,
                                                                         Configuration.ActiveDirectoryInternalPassword)) {
                    string domainUserName = string.Format(Configuration.ActiveDirectoryInternalDomain, userName);

                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, IdentityType.SamAccountName, domainUserName);

                    if (user == null)
                        return false;
                    else
                        try {
                            return user.IsMemberOf(principal, IdentityType.SamAccountName, groupName);
                        } catch {
                            return false;
                        }
                }
            } catch (Exception ex) {
                _logger.WriteErrorLog("Could not get lookup users's role membership", ex);

                return false;
            }
        }

        public List<string> GetAllGroupsUserBelongsTo(UserPrincipal user, List<string> groupNames)
        {
            if (user == null) { throw new ArgumentException("user is required", "user"); }
            if (groupNames.Count == 0 || string.IsNullOrEmpty(groupNames.FirstOrDefault())) { throw new ArgumentException("groupName is required", "groupName"); }

            List<string> returnValue = new List<string>();

            try
            {
                foreach (string g in ((DirectoryEntry)user.GetUnderlyingObject()).Properties["memberOf"])
                {
                    string groupName = g.Substring(3, g.ToLower().IndexOf(",ou=") - 3).ToLower(); // get group name from fully qualified AD string
                    foreach (string s in groupNames)
                    {
                        if (groupName.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                            returnValue.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteErrorLog("Error loading group", ex);
            }

            // If they belong to no groups, return the default: guest
            if (returnValue.Count == 0) {
                returnValue.Add( "guest" );
            }

            return returnValue;
        }

        #endregion
    }
}
