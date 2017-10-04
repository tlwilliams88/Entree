using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Configurations;
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


namespace KeithLink.Svc.Impl.Logic.Configurations {
    public class ExportSettingLogicImpl : IExportSettingLogic {
        #region attributes
        private readonly IExportSettingRepository _exportSettingRepository;
        private readonly IExternalCatalogRepository _externalCatalogRepository;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ctor
        public ExportSettingLogicImpl(IUnitOfWork unitOfWork, IExportSettingRepository exportSettingRepository,
                                                    IExternalCatalogRepository externalCatalogRepository) {
            _unitOfWork = unitOfWork;
            _exportSettingRepository = exportSettingRepository;
            _externalCatalogRepository = externalCatalogRepository;
        }
        #endregion

        #region methods
        private static List<ExportModelConfiguration> DeserializeSettings(string data) {
            BinaryFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(Convert.FromBase64String(data))) {
                return (List<ExportModelConfiguration>)formatter.Deserialize(stream);
            }
        }

        private ExportOptionsModel GetTypeSpecificExportOptions(ExportType type, ListType listType) {
            var options = new ExportOptionsModel();
            options.Fields = new List<ExportModelConfiguration>();

            switch (type) {
                case ExportType.List:
                    if (listType == ListType.Contract) {
                        options.Fields.Add(new ExportModelConfiguration() { Field = "ToDate", Label = "Active To Date" });
                        options.Fields.Add(new ExportModelConfiguration() { Field = "CreatedUTC", Label = "Added Date" });
                        options.Fields.Add(new ExportModelConfiguration() { Field = "ModifiedUTC", Label = "Last Changed Date" });
                    }
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Position", Label = "Line #" });
                    break;
                case ExportType.CartDetail:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Quantity", Label = "Qty" });
                    break;
                case ExportType.Order:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "PONumber", Label = "PO #" });
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
                    options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Label = "Invoice #" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "TypeDescription", Label = "Type" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "StatusDescription", Label = "Status" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceDate", Label = "Invoice Date" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "InvoiceAmount", Label = "Invoice Amount" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "DueDate", Label = "Due Date" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Amount", Label = "Amount" });
                    return options;
                case ExportType.ItemUsage:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "ItemNumber", Order = 1, Label = "Item" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Name", Order = 2, Label = "Name" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Detail", Order = 19, Label = "Detailed Description" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "OrderHistoryString", Order = 20, Label = "Order History" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Class", Order = 21, Label = "Category" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Brand", Order = 22, Label = "Brand" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "ManufacturerName", Order = 23, Label = "Mfr Name" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "UPC", Order = 24, Label = "GTIN" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "VendorItemNumber", Order = 25, Label = "Vendor Item" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "AveragePrice", Order = 26, Label = "Average Price" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "TotalCost", Order = 27, Label = "Total Cost" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "TotalQuantityOrdered", Order = 30, Label = "Qty Ordered" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "TotalQuantityShipped", Order = 40, Label = "Qty Shipped" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Pack", Order = 50, Label = "Pack" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Size", Order = 51, Label = "Size" });
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
                case ExportType.MarketingPreferences:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Email", Order = 1, Label = "Email" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "BranchId", Order = 10, Label = "BranchId" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "CurrentCustomer", Order = 20, Label = "Current Customer?" });

                    options.Fields.Add(new ExportModelConfiguration() { Field = "LearnMore", Order = 21, Label = "Learn More?" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "RegisteredOn", Order = 30, Label = "Date Registered" });
                    return options;
            }

            //Common product detail fields
            options.Fields.Add(new ExportModelConfiguration() { Field = "ItemNumber", Label = "Item" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Name", Label = "Name" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Detail", Label = "Detailed Description" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Favorite", Label = "Favorite" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Description", Label = "Description" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "OrderHistoryString", Label = "Order History" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "BrandExtendedDescription", Label = "Brand" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Pack", Label = "Pack" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Size", Label = "Size" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Each", Label = "Each" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "PackagePrice", Label = "Pack Price" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Notes", Label = "Note" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Category", Label = "Contract Category" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "ItemClass", Label = "Category" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "VendorItemNumber", Label = "Vendor Item #" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "UPC", Label = "UPC" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "Cases", Label = "Cases" });
            options.Fields.Add(new ExportModelConfiguration() { Field = "CasePrice", Label = "Case Price" });
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

            if (type == ExportType.List)
                switch (listType) {
                    case ListType.Favorite:
                        break;
                    case ListType.Custom:
                        options.Fields.Add(new ExportModelConfiguration() { Field = "label", Label = "Label" });
                        options.Fields.Add(new ExportModelConfiguration() { Field = "parlevel", Label = "Par" });
                        break;
                    case ListType.Contract:
                        options.Fields.Add(new ExportModelConfiguration() { Field = "Category", Label = "Contract Category" });
                        options.Fields.Add(new ExportModelConfiguration() { Field = "label", Label = "Label" });
                        break;
                    default:
                        break;

                }

            switch (type) {
                case ExportType.Invoice:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityShipped", Label = "Qty Shipped" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Label = "Qty Ordered" });
                    break;

                case ExportType.OrderDetail:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Label = "# Ordered" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Quantity", Label = "# Confirmed" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "EachYN", Label = "Each" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Price", Label = "Price" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "LineTotal", Label = "Ext Price" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "Status", Label = "Status" });
                    break;
                case ExportType.InvoiceDetail:
                    options.Fields.Add(new ExportModelConfiguration() { Field = "LineNumber", Label = "Line #" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityOrdered", Label = "# Ordered" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "QuantityShipped", Label = "# Shipped" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "ItemPrice", Label = "Price" });
                    options.Fields.Add(new ExportModelConfiguration() { Field = "ExtSalesNet", Label = "Ext Price" });
                    break;
            }

            return options;
        }

        public ExportOptionsModel ReadCustomExportOptions(Guid userId, ExportType type, ListType listType) {
            ExportSetting previousSettings = null;

            if (type == ExportType.List) {
                previousSettings = _exportSettingRepository.Read(u => u.UserId == userId && u.Type == type && u.ListType == listType).FirstOrDefault();
            } else
                previousSettings = _exportSettingRepository.Read(u => u.UserId == userId && u.Type == type).FirstOrDefault();

            List<ExportModelConfiguration> previousOptions = new List<ExportModelConfiguration>();
            var exportOptions = GetTypeSpecificExportOptions(type, listType);

            if (previousSettings != null) {
                previousOptions = DeserializeSettings(previousSettings.Settings);
                exportOptions.SelectedType = previousSettings.ExportFormat;
            }


            //Update the Selected and Order for any used in previous export
            foreach (var setting in previousOptions) {
                var options = exportOptions.Fields.Where(f => f.Field.Equals(setting.Field)).FirstOrDefault();
                if (options != null) {
                    options.Selected = setting.Selected;
                    options.Order = setting.Order;
                }
            }

            return exportOptions;
        }

        public List<ExportExternalCatalog> ReadExternalCatalogs() {
            List<ExportExternalCatalog> externalCatalog = new List<ExportExternalCatalog>();

            foreach (ExternalCatalog externalCatalogItem in _externalCatalogRepository.ReadAll().ToList()) {
                ExportExternalCatalog item = new ExportExternalCatalog();
            
                item.BekBranchId = externalCatalogItem.BekBranchId;
                item.CatalogId = externalCatalogItem.ExternalBranchId;
                item.Type = externalCatalogItem.Type.ToString();
                
                externalCatalog.Add(item);
            }

            return externalCatalog;
        }

        public void SaveUserExportSettings(Guid userId, ExportType type, ListType listType, List<ExportModelConfiguration> configuration, string exportFormat) {
            ExportSetting existingSetting = null;
            if (type == ExportType.List)
                existingSetting = _exportSettingRepository.Read(u => u.UserId == userId && u.Type == type && u.ListType == listType).FirstOrDefault();
            else
                existingSetting = _exportSettingRepository.Read(u => u.UserId == userId && u.Type == type).FirstOrDefault();

            if (existingSetting != null) {
                existingSetting.Settings = SerializeSettings(configuration);
                existingSetting.ExportFormat = exportFormat;
                _exportSettingRepository.Update(existingSetting);
            } else {
                var newSetting = new ExportSetting() {
                    Type = type,
                    ListType = listType,
                    UserId = userId,
                    Settings = SerializeSettings(configuration),
                    ExportFormat = exportFormat
                };
                _exportSettingRepository.Create(newSetting);
            }

            _unitOfWork.SaveChanges();
        }

        private static string SerializeSettings(List<ExportModelConfiguration> configuration) {
            BinaryFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, configuration);
                return Convert.ToBase64String(stream.GetBuffer());
            }
        }
        #endregion
    }
}
