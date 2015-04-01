using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Invoices {
    public interface IImagingLogic {
        List<string> GetInvoiceImages(UserSelectedContext customerInfo, string invoiceNumber);
    }
}
