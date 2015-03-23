using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "UserList")]
    public class ListModel: ICloneable
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

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public ListModel Clone()
		{
			using (var ms = new MemoryStream())
			{
				XmlSerializer xs = new XmlSerializer(typeof(ListModel));
				xs.Serialize(ms, this);
				ms.Position = 0;

				return (ListModel)xs.Deserialize(ms);
			}
		}


		public ListModel ShallowCopy()
		{
			var clonedList = new ListModel()
			{
				BranchId = this.BranchId,
				IsContractList = this.IsContractList,
				IsFavorite = this.IsFavorite,
				IsMandatory = this.IsMandatory,
				IsRecommended = this.IsRecommended,
				IsReminder = this.IsReminder,
				IsShared = this.IsShared,
				IsSharing = this.IsSharing,
				IsWorksheet = this.IsWorksheet,
				ListId = this.ListId,
				Name = this.Name,
				ReadOnly = this.ReadOnly,
				SharedWith = this.SharedWith,
				Type = this.Type,
				Items = new List<ListItemModel>()
			};

			if (this.Items != null)
				foreach (var item in this.Items)
				{
					clonedList.Items.Add(new ListItemModel()
					{
						ListItemId = item.ListItemId,
						Name = item.Name,
						ItemNumber = item.ItemNumber,
						PackSize = item.PackSize,
						Label = item.Label,
						ParLevel = item.ParLevel,
						Position = item.Position,
						TempZone = item.TempZone,
						Category = item.Category,
						FromDate = item.FromDate,
						BrandExtendedDescription = item.BrandExtendedDescription,
						ToDate = item.ToDate,
						CreatedUtc = item.CreatedUtc,
						ModifiedUtc = item.ModifiedUtc,
						CasePrice = item.CasePrice,
						PackagePrice = item.PackagePrice,
                        Each = item.Each
					});
				}

			return clonedList;
		}
	}
}

