using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    class InternalUserDomainRepository : Svc.Core.Profile.IUserDomainRepository
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
                using (PrincipalContext principal = new PrincipalContext(ContextType.Domain, Configuration.ActiveDirectoryInternalServerName))
                {
                    return principal.ValidateCredentials(GetDomainUserName(userName), password, ContextOptions.SimpleBind);
                }
            }
            catch (Exception ex)
            {
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not authenticate user", ex);

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
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not get lookup users's role membership", ex);

                return false;
            }
        }
        
        #endregion

    }
}
