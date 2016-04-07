using System.Text.RegularExpressions;

namespace KeithLink.Svc.Impl.Helpers {
    public static class ProfileHelper {
        public static bool IsInternalAddress(string emailAddress) {
            return Regex.IsMatch(emailAddress, Core.Constants.REGEX_BENEKEITHEMAILADDRESS);
        }
    }
}
