using System;

namespace KeithLink.Svc.Core.Models.Invoices.Imaging.Document {
    public class Info {
        public string id { get; set; }
        public string name { get; set; }
        public Keys keys { get; set; }
        public int version { get; set; }
        public string locationId { get; set; }
    }
}
