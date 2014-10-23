using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "UserList")]
    public class ListModel
    {
        [DataMember(Name = "listid")]
        public long ListId { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "is_contract_list")]
        public bool IsContractList { get; set; }

        [DataMember(Name = "read_only")]
        public bool ReadOnly { get; set; }

        [DataMember(Name = "items")]
        public List<ListItemModel> Items { get; set; }

		[DataMember(Name="isfavorite")]
		public bool IsFavorite { get; set; }

		[DataMember(Name = "isworksheet")]
		public bool IsWorksheet { get; set; }

		public string BranchId { get; set; }
		
		//public string FormattedName(string branchId) 
		//{ 
		//	return string.Format("l{0}_{1}", branchId, Regex.Replace(Name, @"\s+", ""));
		//}

    }
}
