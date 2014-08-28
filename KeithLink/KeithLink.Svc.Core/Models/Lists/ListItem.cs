using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "ListItem")]
    public class ListItem: IComparable
    {
        [DataMember(Name = "listitemid")]
        public Guid ListItemId { get; set; }
        [DataMember(Name = "itemnumber")]
        public string ItemNumber { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "parlevel")]
        public decimal ParLevel { get; set; }
        [DataMember(Name = "position")]
        public int Position { get; set; }
		[DataMember(Name = "packsize")]
		public string PackSize { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[DataMember (Name = "favorite")]
		public bool Favorite { get; set; }
		[DataMember (Name ="storagetemp")]
		public string StorageTemp { get; set; }


		public int CompareTo(object obj)
		{
			return this.Position.CompareTo(((ListItem)obj).Position);
		}
	}
}
