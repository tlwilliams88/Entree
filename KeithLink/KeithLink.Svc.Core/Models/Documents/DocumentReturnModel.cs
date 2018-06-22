using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Documents {
    public class DocumentReturnModel {
        public string name { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public DateTimeOffset? modified { get; set; }
    }
}
