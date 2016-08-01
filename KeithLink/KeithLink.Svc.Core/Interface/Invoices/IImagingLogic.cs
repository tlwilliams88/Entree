using KeithLink.Svc.Core.Models.Invoices.Imaging.Document;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Invoices {
    public interface IImagingLogic {
        List<Base64Image> GetInvoiceImages(UserSelectedContext customerInfo, string invoiceNumber);
    }
}
