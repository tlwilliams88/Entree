﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models
{
	[DataContract]
	[Serializable]
	public class CustomerAccountBalanceModel
	{
		[DataMember(Name = "totalbalance")]
		public decimal TotalBalance { get; set; }
		[DataMember(Name="currentbalance")]
		public decimal CurrentBalance { get; set; }
		[DataMember(Name="pastdue")]
		public decimal PastDue { get; set; }
	}
}
