﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.Invoices;

using System;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Invoices.Imaging.Document;

namespace KeithLink.Svc.Impl.Logic.Invoices
{
    public class ImagingLogicImpl : IImagingLogic
    {
        #region attributes
        private IEventLogRepository _log;
        private IImagingRepository _repo;
        #endregion

        #region ctor
        public ImagingLogicImpl(IImagingRepository imagingRepository, IEventLogRepository eventLogRepository)
        {
            _repo = imagingRepository;
            _log = eventLogRepository;
        }
        #endregion

        #region methods
        /// <summary>
        /// get all images for the specified invoice
        /// </summary>
        /// <param name="customerInfo">the customer's branch and customer number</param>
        /// <param name="invoiceNumber">the customer's invoice number</param>
        /// <returns>all images for the invoice in a base64 string list</returns>
        /// <remarks>
        /// jwames - 3/31/2015 - original code
        /// </remarks>
        public List<Base64Image> GetInvoiceImages(UserSelectedContext customerInfo, string invoiceNumber)
        {
            try
            {
                _log.WriteInformationLog(string.Format("Retrieving invoice images(BranchId: {0}, CustomerNumber: {1}, InvoiceNumber: {2}",
                                                        customerInfo.BranchId, customerInfo.CustomerId, invoiceNumber));
                string sessionToken = _repo.Connect();
                List<string> documentIds = _repo.GetDocumentIds(sessionToken, customerInfo, invoiceNumber);

                List<Base64Image> images = new List<Base64Image>();

                foreach (string documentId in documentIds)
                {
                    images.AddRange(_repo.GetImages(sessionToken, documentId));
                }

                return images;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Unhandled exception while retrieving invoice images.", ex);
                throw;
            }
        }
        #endregion
    }
}