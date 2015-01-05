using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Authentication {
    public enum AuthenticationStatus {
        Successful,
        Locked,
        PasswordExpired,
        Disabled,
        FailedAuthentication,
        FailedConnectingToAuthenticationServer
    }
}
