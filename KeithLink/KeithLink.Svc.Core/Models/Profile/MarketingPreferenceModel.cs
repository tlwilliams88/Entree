using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
	[DataContract]
	public class MarketingPreferenceModel: IExportableModel
	{
		[DataMember(Name = "id")]
		public long Id { get; set; }
		[DataMember(Name = "email")]
		public string Email { get; set; }
		[DataMember(Name = "branchid")]
		public string BranchId { get; set; }
		[DataMember(Name = "iscustomer")]
		public bool CurrentCustomer { get; set; }
		[DataMember(Name = "learnmore")]
		public bool LearnMore { get; set; }
		[DataMember(Name = "registeredon")]
		public DateTime RegisteredOn { get; set; }

		public List<ModelExport.ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "Email", Order = 1, Label = "Email" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "BranchId", Order = 10, Label = "BranchId" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "CurrentCustomer", Order = 20, Label = "Current Customer?" });

			defaultConfig.Add(new ExportModelConfiguration() { Field = "LearnMore", Order = 21, Label = "Learn More?" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "RegisteredOn", Order = 30, Label = "Date Registered" });

			return defaultConfig;
		}
	}
}

