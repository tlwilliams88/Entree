using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ImportFiles;

namespace KeithLink.Svc.Core.Models.Lists {
    public class ListImportFileModel : BaseFileImportModel {

        public ListImportFileModel() {}

        public FileFormat FileFormat { get; set; }

    }
}
