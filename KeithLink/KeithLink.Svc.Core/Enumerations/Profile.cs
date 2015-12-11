using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Profile {
    public enum CustomerSearchType {
        Customer = 1,
        NationalAccount,
        RegionalAccount
    }
    public class SettingKeys
    {
        public static string PageLoadSize = "pageLoadSize";
        public static string Sort = "sortPreferences";
    }
    public class DefaultSetting
    {
        public static string PageLoadSize = "50";
        public static string Sort = "lis4n2nato4n2n";
    }
}
