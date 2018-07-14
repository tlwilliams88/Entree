using Entree.Core.Models.Invoices;
using Entree.Core.Models.ModelExport;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Invoices
{
    public interface IExportInvoicesService
    {
        List<InvoiceItemModel> GetExportableInvoiceItems(UserProfile user,
                                                         UserSelectedContext context,
                                                         ExportRequestModel exportRequest,
                                                         string invoiceNumber,
                                                         Dictionary<string, string> contractdictionary);

        InvoiceModel GetExportableInvoice(UserProfile user,
                                          UserSelectedContext context,
                                          ExportRequestModel exportRequest,
                                          string invoiceNumber);

        List<InvoiceModel> GetExportableInvoiceModels(UserProfile user, 
                                                      UserSelectedContext context, 
                                                      ExportRequestModel request, 
                                                      bool forAllCustomers);
    }
}