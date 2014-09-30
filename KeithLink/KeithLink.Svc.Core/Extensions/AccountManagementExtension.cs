using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Extensions {
    public static class AccountManagementExtension {
        #region methods
        public static string GetCompany(this Principal principal) {
            return principal.GetProperty("company");
        }


        public static string GetProperty(this Principal principal, string property) {
            DirectoryEntry de = (DirectoryEntry)principal.GetUnderlyingObject();

            if (de.Properties.Contains(property))
                return de.Properties[property].Value.ToString();
            else
                return string.Empty;
        }
        #endregion
    }

}
