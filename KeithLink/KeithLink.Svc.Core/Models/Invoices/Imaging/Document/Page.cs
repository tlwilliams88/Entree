using System;

namespace KeithLink.Svc.Core.Models.Invoices.Imaging.Document {
    public class Page {
        public string id { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public int pageNumber { get; set; }
    }
}
