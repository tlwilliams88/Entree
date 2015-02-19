using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ImportFiles {
    public class BaseFileImportModel {
        public BaseFileImportModel() {
            Stream = new System.IO.MemoryStream();
        }

        public string FileName { get; set; }
        public string Contents { get; set; }
        public System.IO.MemoryStream Stream { get; set; }
    }
}
