using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Reports
{
	[DataContract]
	public class InventoryValuationRequestModel
	{
		[DataMember(Name = "format")]
		public string ReportFormat { get; set; }
        [DataMember(Name = "groupby")]
        public string GroupBy { get; set; }
        [DataMember(Name = "data")]
		public List<InventoryValuationModel> ReportData { get; set; }
        public UserSelectedContext context { get; set; }
    }
}
