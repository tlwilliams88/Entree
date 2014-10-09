using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Extensions {
    public static class AccountManagementExtension {
        #region methods
        public static string GetCompany(this Principal principal) {
            return principal.GetProperty("company");
        }

        public static string GetPhoneNumber(this Principal principal) {
            return principal.GetProperty("telephoneNumber");
        }

        public static string GetProperty(this Principal principal, string property) {
            DirectoryEntry de = (DirectoryEntry)principal.GetUnderlyingObject();

            if (de.Properties.Contains(property))
                return de.Properties[property].Value.ToString();
            else
                return string.Empty;
        }

        public static string GetOrganizationalunit(this Principal principal) {
            DirectoryEntry de = (DirectoryEntry)principal.GetUnderlyingObject();

            string[] path = de.Path.Split(',');

            string retVal = null;

            foreach (string splitPath in path) {
                string[] kvp = splitPath.Split('=');

                if (string.Compare(kvp[0], "OU", true) == 0) {
                    retVal = kvp[1];
                    break;
                }
            }

            return retVal;
        }
        #endregion
    }

}
