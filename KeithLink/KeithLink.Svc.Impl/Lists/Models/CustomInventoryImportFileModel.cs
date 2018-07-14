using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Lists.Models.ImportFiles;
using System.Runtime.Serialization;

namespace Entree.Core.Lists.Models {
    [DataContract]
    public class CustomInventoryImportFileModel : BaseFileImportModel {

        public CustomInventoryImportFileModel() {}

        [DataMember(Name = "fileformat")]
        public FileFormat FileFormat { get; set; }

        [DataMember( Name = "ignorefirstline" )]
        public bool IgnoreFirstLine { get; set; }

		[DataMember(Name = "filesource")]
		public bool FileSource { get; set; }
    }
}
