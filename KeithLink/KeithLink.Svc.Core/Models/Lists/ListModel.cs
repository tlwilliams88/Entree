using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.ModelExport;
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

		[DataMember(Name = "sharedwith")]
		public List<string> SharedWith { get; set; }

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

        [DataMember(Name = "isreminder")]
        public bool IsReminder { get; set; }

		[DataMember(Name = "isshared")]
		public bool IsShared { get; set; }

		[DataMember(Name = "issharing")]
		public bool IsSharing { get; set; }

		[DataMember(Name = "ismandatory")]
		public bool IsMandatory { get; set; }

		[DataMember(Name = "isrecommended")]
		public bool IsRecommended { get; set; }
		
        public string BranchId { get; set; }

		[DataMember]
		public ListType Type { get; set; }
			

	}
}

