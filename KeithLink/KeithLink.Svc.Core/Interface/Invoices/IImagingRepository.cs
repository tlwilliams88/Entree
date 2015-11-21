﻿using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Invoices {
    public interface IImagingRepository {
        string Connect();

        List<string> GetDocumentIds(string sessionToken, UserSelectedContext customerInfo, string invoiceNumber);

        List<string> GetImages(string sessionToken, string documentId);
    }
}
