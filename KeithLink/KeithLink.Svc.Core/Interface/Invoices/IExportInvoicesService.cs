using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Invoices
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
                                          string invoiceNumber,
                                          Dictionary<string, string> contractdictionary);
    }
}