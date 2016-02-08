﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract(Name = "NewOrderReturn")]
	public class NewOrderReturn
	{
		[DataMember(Name = "ordernumber")]
		public string OrderNumber { get; set; }

        [DataMember(Name = "catalogType")]
        public string CatalogType { get; set; }

        [DataMember(Name = "isspecialorder")]
        public bool IsSpecialOrder { get; set; }
	}
}
