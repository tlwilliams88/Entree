﻿// KeithLink
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;

// Core
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "ListItem")]
    public class ListItemModel:BaseProductInfo, IComparable, IExportableModel {

        #region properties

        [DataMember(Name = "listitemid")]
        public long ListItemId { get; set; }

        [DataMember(Name = "each")]
        public bool? Each { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "parlevel")]
        public decimal ParLevel { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

		[DataMember(Name = "packsize")]
		[Description("Pack/Size")]
		public string PackSize { get; set; }

		[DataMember(Name = "storagetemp", EmitDefaultValue = false)]
		public string StorageTemp { get; set; }

		[DataMember(Name = "category")]
		[Description("Category")]
		public string Category { get; set; }

		[DataMember(Name = "fromdate")]
		[Description("From Date")]
		public DateTime? FromDate { get; set; }

		[DataMember(Name = "todate")]
		[Description("To Date")]
		public DateTime? ToDate { get; set; }

		[DataMember(Name = "quantity")]
		public decimal Quantity { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime ModifiedUtc { get; set; }

		[DataMember(Name = "isdeleted")]
		public bool IsDelete { get; set; }

		[DataMember]
		public ListType Type { get; set; }

        [DataMember( Name = "itemstatistics" )]
        public ItemHistoryModel ItemStatistics { get; set; }

        [DataMember(Name = "catalog_id")]
        public string CatalogId { get; set; }

        #endregion

        #region functions

        public int CompareTo(object obj)
		{
			return this.Position.CompareTo(((ListItemModel)obj).Position);
		}

		public List<ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Name", Order = 10, Label = "Name" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Brand", Order = 20, Label = "Brand" });

			defaultConfig.Add(new ExportModelConfiguration() { Field = "ItemClass", Order = 21, Label = "Class" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 30, Label = "Pack/Size" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "Notes", Order = 50, Label = "Note" });


			switch (this.Type)
			{
				case ListType.Favorite:
					break;
				case ListType.Custom:
					defaultConfig.Add(new ExportModelConfiguration() { Field = "label", Order = 41, Label = "Label" });
					defaultConfig.Add(new ExportModelConfiguration() { Field = "parlevel", Order = 51, Label = "Par" });
					break;
				case ListType.Contract:
				case ListType.ContractItemsAdded:
				case ListType.ContractItemsDeleted:
					defaultConfig.Add(new ExportModelConfiguration() { Field = "Category", Order = 41, Label = "Category" });
					defaultConfig.Add(new ExportModelConfiguration() { Field = "label", Order = 42, Label = "Label" });
					break;
				default:
					break;

			}

			return defaultConfig;

        }

        #endregion


    }
}
