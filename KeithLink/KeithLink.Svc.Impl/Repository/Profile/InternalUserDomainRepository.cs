using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class InternalUserDomainRepository : Svc.Core.Interface.Profile.IUserDomainRepository
    {
        #region attributes
        IEventLogRepository _logger;
        #endregion

        #region methods

        #region ctor
        public InternalUserDomainRepository(IEventLogRepository logger)
        {
            _logger = logger;
        }
        #endregion
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
        /// authenticate the user and return an error message when authentication fails
        /// </summary>
        /// <param name="userName">the user's network user id</param>
        /// <param name="password">the user's password</param>
        /// <param name="errorMessage">reason for failing authentication</param>
        /// <returns>true/false</returns>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
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
                                                            Configuration.ActiveDirectoryInternalServerName,
                                                            Configuration.ActiveDirectoryInternalRootNode,
                                                            ContextOptions.Negotiate,
                                                            GetDomainUserName(Configuration.ActiveDirectoryInternalUserName),
                                                            Configuration.ActiveDirectoryInternalPassword))
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
                    if (boundServer.ValidateCredentials(userName, password))
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
        /// return the domain and username
        /// </summary>
        /// <param name="userName">the user name without the domain name</param>
        /// <returns>string in the form of domain\username</returns>
        /// <remarks>
        /// jwames - 8/5/2014 - original code
        /// </remarks>
        private string GetDomainUserName(string userName)
        {
            return string.Format("{0}\\{1}", Configuration.ActiveDirectoryInternalDomain, userName);
        }

        /// <summary>
        /// get the user from AD
        /// </summary>
        /// <remarks>
        /// jwames - 8/18/2014 - documented
        /// </remarks>
        public UserPrincipal GetUser(string userName)
        {
            if (userName.Length == 0) { throw new ArgumentException("userName is required", "userName"); }
            if (userName == null) { throw new ArgumentNullException("userName", "userName is null"); }

            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain,
                                                                         Configuration.ActiveDirectoryInternalServerName,
                                                                         Configuration.ActiveDirectoryInternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         GetDomainUserName(Configuration.ActiveDirectoryInternalUserName),
                                                                         Configuration.ActiveDirectoryInternalPassword))
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
        /// get the user from AD
        /// </summary>
        /// <remarks>
        /// jwames - 8/29/2014 - new method
        /// </remarks>
        public UserPrincipal GetUserByEmailAddress(string emailAddress)
        {
            if (emailAddress.Length == 0) { throw new ArgumentException("emailAddress is required", "emailAddress"); }
            if (emailAddress == null) { throw new ArgumentNullException("emailAddress", "userName is null"); }

            return GetUser(emailAddress.Split('@')[0]);
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
                                                                         Configuration.ActiveDirectoryInternalServerName,
                                                                         Configuration.ActiveDirectoryInternalRootNode,
                                                                         ContextOptions.Negotiate,
                                                                         GetDomainUserName(Configuration.ActiveDirectoryInternalUserName),
                                                                         Configuration.ActiveDirectoryInternalPassword))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(principal, GetDomainUserName(userName));

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
                _logger.WriteErrorLog("Could not get lookup users's role membership", ex);

                return false;
            }
        }
        
        #endregion
    }
}
