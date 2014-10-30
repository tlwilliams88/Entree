﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	public enum ItemNumberType
	{
		ItemNumber,
		UPC,
		ItemNumberOrUPC
	}

	public enum FileContentType
	{
		ItemOnly,
		ItemQty
	}

	public enum FileFormat
	{
		CSV,
		Tab,
		Excel
	}


	[DataContract]
	public class OrderImportOptions
	{
		[DataMember(Name="itemnumber")]
		public ItemNumberType ItemNumber { get; set; }
		[DataMember(Name = "contents")]
		public FileContentType Contents { get; set; }
		[DataMember(Name = "fileformat")]
		public FileFormat FileFormat { get; set; }
		[DataMember(Name = "ignorezero")]
		public bool IgnoreZeroQuantities { get; set; }
		[DataMember(Name ="ignorefirst")]
		public bool IgnoreFirstLine { get; set; }

	}

}