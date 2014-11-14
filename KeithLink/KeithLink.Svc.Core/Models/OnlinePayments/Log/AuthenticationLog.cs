using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log {
    public class AuthenticationLog {
        public long LogId { get; set; }

        public DateTime AuthenticationTime { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }
    }
}
