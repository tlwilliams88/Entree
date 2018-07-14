﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;

namespace Entree.Core.Lists.Models
{
	[DataContract]
	public class ListImportModel
	{
        [DataMember(Name = "list")]
        public ListModel List { get; set; }
        [DataMember(Name="success")]
        public bool Success { get; set; }
		[DataMember(Name="errormsg")]
        public string ErrorMessage { get; set; }
        [DataMember(Name="warningmsg")]
        public string WarningMessage { get; set; }
	}
}
