using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KeithLink.Svc.Impl.Helpers {
    public static class ProfileHelper {
        public static bool IsInternalAddress(string emailAddress) {
            return Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS);
        }

        public static bool CheckCanViewUNFI(UserProfile user, string customernumber, string customerbranch, ICustomerRepository customerRepository, IEventLogRepository _log)
        {
            if ((user != null) &&
                (user.IsInternalUser) &&
                (KeithLink.Svc.Impl.Configuration.WhiteListedUNFIBEKUsers.Contains(user.UserName, StringComparer.CurrentCultureIgnoreCase)))
            {
                return true;
            }
            if ((user != null) &&
                (user.RoleName.Equals("dsr", StringComparison.CurrentCultureIgnoreCase)) &&
                (KeithLink.Svc.Impl.Configuration.WhiteListedUNFIDSRs.Contains(user.UserName, StringComparer.CurrentCultureIgnoreCase)))
            {
                return true;
            }
            if (customernumber != null && customerbranch != null)
            {
                if (KeithLink.Svc.Impl.Configuration.WhiteListedUNFIBranches.Contains(customerbranch, StringComparer.CurrentCultureIgnoreCase))
                {
                    return true;
                }

                Customer existingCustomer = customerRepository.GetCustomerByCustomerNumber(customernumber, customerbranch);

                if (existingCustomer == null)
                {
                    _log.WriteWarningLog(string.Format("customer lookup; customer or its DSR are null with {0} on branch {1}", customernumber, customerbranch));
                }
                else
                {
                    if (existingCustomer.Dsr == null)
                    {
                        _log.WriteWarningLog(string.Format("customer lookup; customer or its DSR are null with {0} on branch {1}", customernumber, customerbranch));
                    }
                }

                if (existingCustomer != null && existingCustomer.Dsr != null && existingCustomer.Dsr.Name != null &&
                    KeithLink.Svc.Impl.Configuration.WhiteListedUNFIDSRs.Contains(existingCustomer.Dsr.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    return true;
                }

                if (KeithLink.Svc.Impl.Configuration.WhiteListedUNFICustomers.Contains(customernumber))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
