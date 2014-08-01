using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace KeithLink.Svc.Impl.Profile
{
    public static class AuthenticationManager 
    {
        public bool AuthenticateUser(string userName, string password)
        {
            try
            {
                using (PrincipalContext principal = new PrincipalContext(ContextType.ApplicationDirectory, 
                                                                         Configuration.ActiveDirectoryServerName, 
                                                                         Configuration.ActiveDirectoryRootNode,
                                                                         ContextOptions.SecureSocketLayer | ContextOptions.SimpleBind, 
                                                                         Configuration.ActiveDirectoryUserName, 
                                                                         Configuration.ActiveDirectoryPassword) )
                {
                    return principal.ValidateCredentials(userName, password);                
                }
            }
            catch (Exception ex){
                new Common.Impl.Logging.EventLogRepositoryImpl(Configuration.ApplicationName).WriteErrorLog("Could not authenticate user", ex);

                return false;
            }
        }
    }
}
