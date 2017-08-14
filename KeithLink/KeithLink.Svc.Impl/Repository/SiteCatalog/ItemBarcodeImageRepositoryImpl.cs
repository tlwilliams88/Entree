using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BarcodeLib;

using KeithLink.Svc.Core.Extensions.Reports;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class ItemBarcodeImageRepositoryImpl : IItemBarcodeImageRepository
    {
        #region attributes
        private const string ITEMNUMBER_BARCODE_FORMAT = "*{0}*";
        #endregion
        #region methods
        public List<ItemBarcodeModel> GetBarcodeForList(ListModel list)
        {
            if (list == null) {
                return null;
            }

            List<ItemBarcodeModel> returnBarcodeModels = new List<ItemBarcodeModel>();

            if (list.Items != null) {
                foreach (var listItem in list.Items)
                {
                    returnBarcodeModels.Add(listItem.ToItemBarcodeModel(GetBarcode(string.Format(ITEMNUMBER_BARCODE_FORMAT, listItem.ItemNumber))));
                }
            }

            return returnBarcodeModels;
        }

        private byte[] GetBarcode(string text)
        {
            Bitmap b;
            Barcode bar = new Barcode(text);
            bar.Alignment = AlignmentPositions.LEFT;
            bar.IncludeLabel = false;
            bar.RotateFlipType = RotateFlipType.RotateNoneFlipNone;
            b = (Bitmap)bar.Encode(TYPE.CODE39Extended, text, 250, 40);
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                b.Save(ms, ImageFormat.Bmp);
                data = ms.ToArray();
            }
            return data;
        }
        #endregion
    }
}
