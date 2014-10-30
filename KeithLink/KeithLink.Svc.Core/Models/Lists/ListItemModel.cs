using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "ListItem")]
    public class ListItemModel:BaseProductInfo, IComparable
    {
        [DataMember(Name = "listitemid")]
        public long ListItemId { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "parlevel")]
        public decimal ParLevel { get; set; }
        [DataMember(Name = "position")]
        public int Position { get; set; }
		[DataMember(Name = "packsize")]
		public string PackSize { get; set; }
		[DataMember(Name = "storagetemp")]
		public string StorageTemp { get; set; }
		[DataMember(Name = "quantityincart")]
		public decimal? QuantityInCart { get; set; }
		[DataMember(Name = "category")]
		public string Category { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }

        public KeithLink.Svc.Core.Enumerations.List.ListItemStatus Status { get; set; }

		public int CompareTo(object obj)
		{
			return this.Position.CompareTo(((ListItemModel)obj).Position);
		}
	}
}
