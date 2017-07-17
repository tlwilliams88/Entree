using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;

namespace KeithLink.Svc.Core.Interface.Reports
{
    public interface IItemBarcodeImageRepository {
        List<ItemBarcodeModel> GetBarcodeForList(ListModel list);
    }
}
