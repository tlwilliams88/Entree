using KeithLink.Svc.Core.Helpers;
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
            retVal.ItemPrice = (decimal)value.Price; // for an invoiceitem, we're not figuring a price we use the set price of the item.
            retVal.QuantityShipped = value.QantityShipped;
            retVal.QuantityOrdered = value.QuantityOrdered;
            retVal.LineNumber = value.LineNumber.ToString();
            retVal.IsValid = value.IsValid;
            retVal.Name = value.Name;
            retVal.Description = value.Description;
            retVal.PackSize = value.PackSize;
            retVal.Pack = value.Pack;
            retVal.Each = value.Each;
            retVal.Brand = value.BrandExtendedDescription;
            retVal.BrandExtendedDescription = value.BrandExtendedDescription;
            retVal.ReplacedItem = value.ReplacedItem;
            retVal.ReplacementItem = value.ReplacementItem;
            retVal.NonStock = value.NonStock;
            retVal.ChildNutrition = value.ChildNutrition;
            retVal.CatchWeight = value.CatchWeight;
            retVal.TempZone = value.TempZone;
            retVal.ItemClass = value.ItemClass;
            retVal.CategoryCode = value.CategoryCode;
            retVal.SubCategoryCode = value.SubCategoryCode;
            retVal.CategoryName = value.CategoryName;
            retVal.UPC = value.UPC;
            retVal.VendorItemNumber = value.VendorItemNumber;
            retVal.Cases = value.Cases;
            retVal.Kosher = value.Kosher;
            retVal.ManufacturerName = value.ManufacturerName;
            retVal.ManufacturerNumber = value.ManufacturerNumber;
            retVal.Nutritional = value.Nutritional;
            retVal.ExtCatchWeight = value.TotalShippedWeight;
            if (retVal.CatchWeight)
            {
                retVal.ExtSalesNet = retVal.ExtCatchWeight * retVal.ItemPrice;
            }
            else
            {
                retVal.ExtSalesNet = retVal.QuantityShipped * retVal.ItemPrice;
            }

            return retVal;
        }
    }
}
