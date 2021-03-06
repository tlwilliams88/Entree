﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract]
	public class OrderImportModel
	{
		[DataMember(Name = "listid")]
		public Guid? ListId { get; set; }
		[DataMember(Name = "success")]
		public bool Success { get; set; }
		[DataMember(Name = "errormsg")]
		public string ErrorMessage { get; set; }
		[DataMember(Name = "warningmsg")]
		public string WarningMessage { get; set; }
        [DataMember(Name = "successmsg")]
        public string SuccessMessage { get; set; }
    }
}
