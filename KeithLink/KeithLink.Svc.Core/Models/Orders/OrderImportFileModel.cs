using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders {
    public class OrderImportFileModel {

        public OrderImportFileModel() {
            Stream = new System.IO.MemoryStream();
            Options = new OrderImportOptions();
        }
        
        public string FileName { get; set; }
        public string Contents { get; set; }
        public System.IO.MemoryStream Stream { get; set; }
        public OrderImportOptions Options { get; set; }

    }
}
