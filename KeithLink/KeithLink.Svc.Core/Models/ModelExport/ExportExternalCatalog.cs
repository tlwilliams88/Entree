using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ModelExport
{
	[DataContract]
	public class ExportExternalCatalog
	{
        [DataMember(Name = "BekBranchId")]
        public string BekBranchId { get; set; }
        [DataMember(Name = "ExternalBranchId")]
        public string CatalogId { get; set; }
        [DataMember(Name = "Type")]
        public string Type { get; set; }
	}
}
