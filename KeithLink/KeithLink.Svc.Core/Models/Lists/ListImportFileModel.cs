using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.ImportFiles;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Lists {
    [DataContract]
    public class ListImportFileModel : BaseFileImportModel {

        public ListImportFileModel() {}

        [DataMember(Name = "fileformat")]
        public FileFormat FileFormat { get; set; }

        [DataMember( Name = "ignorefirstline" )]
        public bool IgnoreFirstLine { get; set; }

		[DataMember(Name = "filesource")]
		public bool FileSource { get; set; }
    }
}
