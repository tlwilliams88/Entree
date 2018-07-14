using Entree.Core.Models.Invoices.Imaging.Document;
using Entree.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.Invoices {
    public interface IImagingLogic {
        List<Base64Image> GetInvoiceImages(UserSelectedContext customerInfo, string invoiceNumber);
    }
}
