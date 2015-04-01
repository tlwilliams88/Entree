using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Models.Invoices.Imaging.Document {
    public class ImageNowDocumentReturnModel {
        public Info info { get; set; }
        public List<object> workflowItems { get; set; }
        public List<Page> pages { get; set; }
        public List<object> properties { get; set; }
    }
}
