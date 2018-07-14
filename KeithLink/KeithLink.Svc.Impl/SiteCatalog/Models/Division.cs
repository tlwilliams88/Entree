﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models;

namespace Entree.Core.SiteCatalog.Models
{
	[DataContract]
	public class Division
	{
		[DataMember(Name="id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "branchsupport")]
		public BranchSupportModel BranchSupport { get; set; }
	}
}
