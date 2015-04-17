using System;
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
		ItemQty,
        ItemQtyBrokenCase
	}


	[DataContract]
	public class OrderImportOptions
	{
		[DataMember(Name="itemnumber")]
		public ItemNumberType ItemNumber { get; set; }

		[DataMember(Name = "contents")]
		public FileContentType Contents { get; set; }

		[DataMember(Name = "fileformat")]
		public KeithLink.Svc.Core.Models.ImportFiles.FileFormat FileFormat { get; set; }

		[DataMember(Name = "ignorezero")]
		public bool IgnoreZeroQuantities { get; set; }

		[DataMember(Name ="ignorefirst")]
		public bool IgnoreFirstLine { get; set; }

        [DataMember( Name = "importbyinventory" )]
        public bool ImportByInventory { get; set; }

		[DataMember( Name = "listid")]
		public long? ListId { get; set; }

	}

}
