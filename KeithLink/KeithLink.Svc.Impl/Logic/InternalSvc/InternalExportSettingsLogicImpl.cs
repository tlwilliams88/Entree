using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalExportSettingsLogicImpl:IInternalExportSettingLogic
	{
		private readonly IExportSettingRepository exportSettingRepository;
		private readonly IListRepository listRepository;
		private readonly IUnitOfWork unitOfWork;

		public InternalExportSettingsLogicImpl(IUnitOfWork unitOfWork, IExportSettingRepository exportSettingRepository, IListRepository listRepository)
		{
			this.unitOfWork = unitOfWork;
			this.exportSettingRepository = exportSettingRepository;
			this.listRepository = listRepository;
		}

		public ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, long? ListId)
		{
			ExportSetting previousSettings = null;
			ListType listType = ListType.Custom;

			if (type == ExportType.List)
			{
				//Determine the list type
				var list = listRepository.ReadById(ListId.Value);

				if (list == null)
					return null;

				listType = list.Type;

				previousSettings = exportSettingRepository.Read(u => u.UserId == userId && u.Type == type && u.ListType == list.Type).FirstOrDefault();
			}
			else
				previousSettings = exportSettingRepository.Read(u => u.UserId == userId && u.Type == type).FirstOrDefault();

			List<ExportModelConfiguration> previousOptions = new List<ExportModelConfiguration>();
			var exportOptions = GetTypeSpecificExportOptions(type, listType);

			if (previousSettings != null)
			{
				previousOptions = DeserializeSettings(previousSettings.Settings);
				exportOptions.SelectedType = previousSettings.ExportFormat;
			}

			
			//Update the Selected and Order for any used in previous export
			foreach (var setting in previousOptions)
			{
				var options = exportOptions.Fields.Where(f => f.Field.Equals(setting.Field)).FirstOrDefault();
				if (options != null)
				{
					options.Selected = setting.Selected;
					options.Order = setting.Order;
				}
			}


			return exportOptions;
		}

		private ExportOptionsModel GetTypeSpecificExportOptions(ExportType type, ListType listType)
		{
			var options = new ExportOptionsModel();
			options.Fields = new List<ExportModelConfiguration>();

			switch (type)
			{
				case ExportType.Order:
					options.Fields.Add(new ExportModelConfiguration() { Field = "OrderNumber", Label = "Order #" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "CreatedDate", Label = "Order Date" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Status", Label = "Status" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "DeliveryDate", Label = "Delivery Date" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "ItemCount", Label = "Item Count" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "OrderTotal", Label = "Total" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Label = "Invoice #" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceStatus", Label = "Invoice Status" });
					return options;
				case ExportType.Invoice:
					options.Fields.Add(new ExportModelConfiguration() { Field = "CustomerNumber", Label = "Customer Number" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "CustomerName", Label = "Customer Name" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceNumber",Label = "Invoice #" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "TypeDescription", Label = "Type" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "StatusDescription", Label = "Status" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "DueDate", Label= "Due Date" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Amount", Label = "Amount" });
					return options;
				case ExportType.ItemUsage:

					options.Fields.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Name", Order = 20, Label = "Name" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Brand", Order = 21, Label = "Brand" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "ManufacturerName", Order = 21, Label = "Mfr Name" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "UPC", Order = 22, Label = "GTIN" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "VendorItemNumber", Order = 23, Label = "Vendor Item" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "AveragePrice", Order = 24, Label = "Average Price" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "TotalCost", Order = 25, Label = "Total Cost" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "TotalQuantityOrdered", Order = 30, Label = "Qty Ordered" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "TotalQuantityShipped", Order = 40, Label = "Qty Shipped" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "PackSize", Order = 50, Label = "Pack / Size" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Each", Order = 60, Label = "Each" });
					return options;
				case ExportType.PendingTransactions:
					options.Fields.Add(new ExportModelConfiguration() { Field = "CustomerName", Order = 1, Label = "Customer Name" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "CustomerNumber", Order = 10, Label = "Customer Number" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "ConfirmationId", Order = 20, Label = "Confirmation Id" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Order = 30, Label = "Invoice #" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "PaymentAmount", Order = 40, Label = "Amount" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "AccountNumber", Order = 50, Label = "Account" });

					return options;
			}
				

			//Common product detail fields
			options.Fields.Add(new ExportModelConfiguration() { Field = "ItemNumber", Label = "Item" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Name", Label = "Name" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Favorite", Label = "Favorite" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Description", Label = "Description" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Label = "Brand" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "PackSize", Label = "Pack/Size" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Notes", Label = "Note" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "CategoryId", Label = "Category" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "CategoryName", Label = "Category Desc" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "ItemClass", Label = "Class" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "VendorItemNumber", Label = "Vendor Item #" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "UPC", Label = "UPC" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Cases", Label = "Cases" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "CatchWeight", Label = "Catch Weight" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.CountryOfOrigin", Label = "Country Of Origin" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.GrossWeight", Label = "Gross Weight" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.HandlingInstructions", Label = "Handling Instruction" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.Height", Label = "Height" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.Length", Label = "Length" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.Width", Label = "Width" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Nutritional.Ingredients", Label = "Ingredients" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "Kosher", Label = "Kosher" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "ManufacturerName", Label = "Manufacturer Name" });
			options.Fields.Add(new ExportModelConfiguration() { Field = "ManufacturerNumber", Label = "Manufacturer Number" });
			
			
			

			if(type == ExportType.List)
				switch (listType)
				{
					case ListType.Favorite:
						break;
					case ListType.Custom:
						options.Fields.Add(new ExportModelConfiguration() { Field = "label", Label = "Label" });
						options.Fields.Add(new ExportModelConfiguration() { Field = "parlevel", Label = "Par" });
						break;
					case ListType.Contract:
					case ListType.ContractItemsAdded:
					case ListType.ContractItemsDeleted:
						options.Fields.Add(new ExportModelConfiguration() { Field = "Category", Label = "Category" });
						options.Fields.Add(new ExportModelConfiguration() { Field = "label", Label = "Label" });
						break;
					default:
						break;

				}

			switch (type)
			{
				case ExportType.Invoice:
					options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityShipped", Label = "Qty Shipped" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Label = "Qty Ordered" });
					break;
				
				case ExportType.OrderDetail:
					options.Fields.Add(new ExportModelConfiguration() { Field = "Quantity", Label = "# Requested" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Label = "# Ordered" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "QantityShipped", Label = "# Shipped" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "EachYN", Label = "Each" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Price", Label = "Price" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "Status", Label = "Status" });
					break;
				case ExportType.InvoiceDetail:
					options.Fields.Add(new ExportModelConfiguration() { Field = "LineNumber", Label= "Line #" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered",Label = "# Ordered" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityShipped", Label = "# Shipped" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "ItemPrice", Label = "Price" });
					options.Fields.Add(new ExportModelConfiguration() { Field = "ExtSalesNet", Label ="Ext Price" });
					break;
			}

			return options;
		}

		public void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat)
		{
			ExportSetting existingSetting = null;
			if (type == ExportType.List)
				existingSetting = exportSettingRepository.Read(u => u.UserId == userId && u.Type == type && u.ListType == listType).FirstOrDefault();
			else
				existingSetting = exportSettingRepository.Read(u => u.UserId == userId && u.Type == type).FirstOrDefault();

			if (existingSetting != null)
			{
				existingSetting.Settings = SerializeSettings(configuration);
				existingSetting.ExportFormat = exportFormat;
				exportSettingRepository.Update(existingSetting);
			}
			else
			{
				var newSetting = new ExportSetting() { 
					Type = type,
					ListType = listType,
					UserId = userId,
					Settings = SerializeSettings(configuration),
					ExportFormat = exportFormat
				};
				exportSettingRepository.Create(newSetting);
			}

			unitOfWork.SaveChanges();
		}

		#region Helper Methods
		private static string SerializeSettings(List<ExportModelConfiguration> configuration)
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, configuration);
				return Convert.ToBase64String(stream.GetBuffer());
			}
		}

		private static List<ExportModelConfiguration> DeserializeSettings(string data)
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (var stream = new MemoryStream(Convert.FromBase64String(data)))
			{
				return (List<ExportModelConfiguration>)formatter.Deserialize(stream);
			}
		}
		#endregion
	}
}
