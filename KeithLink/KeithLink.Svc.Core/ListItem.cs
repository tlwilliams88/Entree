using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
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
        public int ParLevel { get; set; }
        [DataMember(Name = "position")]
        public int Position { get; set; }

		public int CompareTo(object obj)
		{
			return this.Position.CompareTo(((ListItem)obj).Position);
		}
	}
}
