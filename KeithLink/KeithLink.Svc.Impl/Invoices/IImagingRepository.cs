using Entree.Core.Models.Invoices.Imaging.Document;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.Invoices {
    public interface IImagingRepository {
        string Connect();

        List<string> GetDocumentIds(string sessionToken, UserSelectedContext customerInfo, string invoiceNumber);

        List<Base64Image> GetImages(string sessionToken, string documentId);
    }
}
