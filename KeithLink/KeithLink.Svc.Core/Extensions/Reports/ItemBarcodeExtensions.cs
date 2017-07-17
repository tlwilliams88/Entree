using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Extensions.Reports
{
    public static class ItemBarcodeExtensions
    {
        #region methods
        public static ItemBarcodeModel ToItemBarcodeModel(this ListItemModel item, byte[] barcode)
        {
            return new ItemBarcodeModel()
            {
                ItemNumber = item.ItemNumber,
                Name = item.Name,
                PackSize = item.PackSize,
                BarCode = barcode
            };
        }
        #endregion
    }
}
