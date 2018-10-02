﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Core.Models.ModelExport
{
	[DataContract]
	public class ExportRequestModel
	{
        [DataMember(Name="fields")]
        public List<ExportModelConfiguration> Fields { get; set; }
        [DataMember(Name = "selectedtype")]
        public string SelectedType { get; set; }
        [DataMember(Name = "sort")]
        public List<SortInfo> Sort { get; set; }
        [DataMember(Name = "filter")]
        public FilterInfo Filter { get; set; }
	    [DataMember(Name = "search")]
	    public FilterInfo Search { get; set; }
        [DataMember(Name = "daterange")]
        public FilterInfo DateRange { get; set; }
    }
}
