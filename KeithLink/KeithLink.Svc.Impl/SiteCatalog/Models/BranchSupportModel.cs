﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.SiteCatalog.Models
{
	[DataContract(Name = "BranchSupport")]
	public class BranchSupportModel
	{
		[DataMember(Name = "branchid")]
		public string BranchId { get; set; }
		[DataMember(Name = "branchname")]
		public string BranchName { get; set; }
		[DataMember(Name = "supportphonenumber")]
		public string SupportPhoneNumber { get; set; }
		[DataMember(Name = "tollfreenumber")]
		public string TollFreeNumber { get; set; }
		[DataMember(Name = "email")]
		public string Email { get; set; }

	}
}
