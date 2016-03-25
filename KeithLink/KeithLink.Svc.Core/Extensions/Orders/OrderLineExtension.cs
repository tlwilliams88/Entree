using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Orders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders {
    public static class OrderLineExtension {
        public static InvoiceItemModel ToInvoiceItem(this OrderLine value) {

            InvoiceItemModel retVal = new InvoiceItemModel();

            retVal.ItemNumber = value.ItemNumber;
            retVal.ItemPrice = value.Each ? (decimal)value.PackagePriceNumeric : (decimal)value.CasePriceNumeric;
            retVal.QuantityShipped = value.QantityShipped;
            retVal.QuantityOrdered = value.QuantityOrdered;
            retVal.LineNumber = value.LineNumber.ToString();
            retVal.ExtSalesNet = retVal.QuantityShipped * retVal.ItemPrice;
            retVal.IsValid = value.IsValid;
            retVal.Name = value.Name;
            retVal.Description = value.Description;
            retVal.PackSize = value.PackSize;
            retVal.Brand = value.BrandExtendedDescription;
            retVal.BrandExtendedDescription = value.BrandExtendedDescription;
            retVal.ReplacedItem = value.ReplacedItem;
            retVal.ReplacementItem = value.ReplacementItem;
            retVal.NonStock = value.NonStock;
            retVal.ChildNutrition = value.ChildNutrition;
            retVal.CatchWeight = value.CatchWeight;
            retVal.TempZone = value.TempZone;
            retVal.ItemClass = value.ItemClass;
            retVal.CategoryId = value.CategoryId;
            retVal.CategoryName = value.CategoryName;
            retVal.UPC = value.UPC;
            retVal.VendorItemNumber = value.VendorItemNumber;
            retVal.Cases = value.Cases;
            retVal.Kosher = value.Kosher;
            retVal.ManufacturerName = value.ManufacturerName;
            retVal.ManufacturerNumber = value.ManufacturerNumber;
            retVal.Nutritional = value.Nutritional;

            return retVal;
        }
    }
}
