using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Reports;

namespace Entree.Core.Interface.Reports
{
    public interface IItemBarcodeImageRepository {
        List<ItemBarcodeModel> GetBarcodeForList(ListModel list);
    }
}
