using Entree.Core.Models.Lists;
using Entree.Core.Models.Orders;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface
{
    public interface IImportLogic {
        OrderImportModel ImportOrder(UserProfile user, UserSelectedContext catalogInfo, OrderImportFileModel file);

        CustomInventoryImportModel ImportCustomInventory
                (UserProfile user, UserSelectedContext catalogInfo, CustomInventoryImportFileModel file);
    }
}
