using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Authentication;

namespace KeithLink.Svc.Core.Models.Authentication {
    public class AuthenticationModel {
        public string Message { get; set; }
        public AuthenticationStatus Status { get; set; } 
    }
}
