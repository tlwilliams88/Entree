using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments {
    public class State {
        public string StateId { get; set; }

        public string Name { get; set; }
    }
}
