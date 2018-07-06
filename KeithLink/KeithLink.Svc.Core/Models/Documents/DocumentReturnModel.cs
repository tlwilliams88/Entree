using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Documents {
    public class DocumentReturnModel {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
