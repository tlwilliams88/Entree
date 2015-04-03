﻿using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices.Imaging.Document;
using KeithLink.Svc.Core.Models.Invoices.Imaging.View;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Invoices {
    public class ImagingRepositoryImpl : IImagingRepository {
        #region attributes
        const string API_ENDPOINT_CONNECTION = "connection";
        const string API_ENDPOINT_DOCUMENT = "document";
        const string API_ENDPOINT_VIEW = "view";

        private IEventLogRepository _log;
        #endregion

        #region ctor
        public ImagingRepositoryImpl(IEventLogRepository eventLogRepo) {
            _log = eventLogRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// connect to the imaging server using attributes from the config file
        /// </summary>
        /// <returns>return the session token needed for subsequent calls</returns>
        /// <remarks>
        /// jwames = 3/27/2015 - original code
        /// </remarks>
        public string Connect() {
            using (HttpClient client = new HttpClient()) {
                if (Configuration.ImagingUserName.Length == 0) { throw new ApplicationException("No username supplied for ImageNow Integration Server"); }
                if (Configuration.ImagingUserPassword.Length == 0) { throw new ApplicationException("No password supplied for ImageNow Integration Server"); }

                client.DefaultRequestHeaders.Add(Constants.IMAGING_HEADER_USERNAME, Configuration.ImagingUserName);
                client.DefaultRequestHeaders.Add(Constants.IMAGING_HEADER_PASSWORD, Configuration.ImagingUserPassword);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                try {
                    string endPoint = string.Concat(Configuration.ImagingServerUrl, API_ENDPOINT_CONNECTION);

                    System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                    if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        IEnumerable<string> tokenValues = new List<string>();
                        
                        if(response.Headers.TryGetValues(Constants.IMAGING_HEADER_SESSIONTOKEN, out tokenValues)){
                            return tokenValues.ToList()[0];
                        } else {
                            throw new ApplicationException("Connecting to Imaging Server failed to return a session token");
                        }
                    } else {
                        throw new ApplicationException("Could not connect to Imaging Server");
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error connecting to the Imaging Server", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// search the imaging system for our invoice to get the document's unique identifier
        /// </summary>
        /// <param name="sessionToken">the token received from logging into the integration server</param>
        /// <param name="customerInfo">branch and customer information</param>
        /// <param name="invoiceNumber">the customer's invoice number</param>
        /// <returns>the document's unique identifier within the imaging system</returns>
        /// <remarks>
        /// jwames - 3/30/2015 - original code
        /// </remarks>
        public string GetDocumentId(string sessionToken, UserSelectedContext customerInfo, string invoiceNumber) {
            if (sessionToken.Length == 0) { throw new ArgumentException("SessionToken cannot be blank. Reauthentication might be necessary."); }
            if (customerInfo.BranchId.Length == 0) { throw new ArgumentException("Branch cannot be blank"); }
            if (customerInfo.CustomerId.Length == 0) { throw new ArgumentException("Customer number cannot be blank"); }
            if (invoiceNumber.Length == 0) { throw new ArgumentException("Invoice number cannot be blank"); }

            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Add(Constants.IMAGING_HEADER_SESSIONTOKEN, sessionToken);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                try {
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("vslText", string.Format("[drawer] = '{0}AR501' AND [tab] = '{1}' AND [f4] = '{2}'", customerInfo.BranchId, customerInfo.CustomerId, invoiceNumber));
                
                    string endPoint = string.Format("{0}{1}/{2}/result?category=DOCUMENT", Configuration.ImagingServerUrl, API_ENDPOINT_VIEW, Configuration.ImagingViewId);

                    System.Net.Http.HttpResponseMessage response = client.PostAsJsonAsync(endPoint, values).Result;

                    if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        string rawJson = response.Content.ReadAsStringAsync().Result;
                        ImageNowViewQueryReturnModel jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageNowViewQueryReturnModel>(rawJson);

                        string docId = jsonResponse.resultRows[0].fields.Where(item => item.columnId.Equals("8")).FirstOrDefault().value;

                        return docId;
                    } else {
                        throw new ApplicationException("Document not found");
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error connecting to the Imaging Server", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// get base64 strings of all of the images for the specified document
        /// </summary>
        /// <param name="sessionToken">the token received from logging into the integration server</param>
        /// <param name="documentId">the document's unique identifier within the imaging system</param>
        /// <returns>all pages of the document in a base64 string list</returns>
        /// <remarks>
        /// jwames - 3/31/2015 - original code
        /// </remarks>
        public List<string> GetImages(string sessionToken, string documentId) {
            if (sessionToken.Length == 0) { throw new ArgumentException("SessionToken cannot be blank. Reauthentication might be necessary."); }
            if (documentId.Length == 0) { throw new ArgumentException("DocumentId cannot be blank."); }

            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Add(Constants.IMAGING_HEADER_SESSIONTOKEN, sessionToken);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                try {
                    string endPoint = string.Format("{0}{1}/{2}/page", Configuration.ImagingServerUrl, API_ENDPOINT_DOCUMENT, documentId);

                    System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                    if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        string rawJson = response.Content.ReadAsStringAsync().Result;
                        ImageNowPageReturnModel jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageNowPageReturnModel>(rawJson);

                        List<string> retVal = new List<string>();

                        foreach (Page imagePage in jsonResponse.pages) {
                            retVal.Add(GetImageString(sessionToken, documentId, imagePage.id));
                        }

                        return retVal;
                    } else {
                        throw new ApplicationException("Document not found");
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error connecting to the Imaging Server", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// get the image for the specified page as a base64 string
        /// </summary>
        /// <param name="sessionToken">the token received from logging into the integration server</param>
        /// <param name="documentId">the document's unique identifier within the imaging system</param>
        /// <param name="pageId">the current page's unique identifier</param>
        /// <returns>the image data for the specified page in a base64 string</returns>
        /// <remarks>
        /// jwames - 3/31/2015 - original code
        /// </remarks>
        private string GetImageString(string sessionToken, string documentId, string pageId) {
            if (sessionToken.Length == 0) { throw new ArgumentException("SessionToken cannot be blank. Reauthentication might be necessary."); }
            if (documentId.Length == 0) { throw new ArgumentException("DocumentId cannot be blank."); }
            if (pageId.Length == 0) { throw new ArgumentException("PageId cannot be blank."); }
            
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Add(Constants.IMAGING_HEADER_SESSIONTOKEN, sessionToken);
                
                try {
                    string endPoint = string.Format("{0}{1}/{2}/page/{3}/preview", Configuration.ImagingServerUrl, API_ENDPOINT_DOCUMENT, documentId, pageId);

                    System.Net.Http.HttpResponseMessage response = client.GetAsync(endPoint).Result;

                    if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        using (System.IO.MemoryStream tempImgStream = new System.IO.MemoryStream()) {
                            //System.Drawing.Bitmap.FromStream(response.Content.ReadAsStreamAsync().Result).Save(tempImgStream, System.Drawing.Imaging.ImageFormat.Png);
                            System.Drawing.Bitmap.FromStream(response.Content.ReadAsStreamAsync().Result).Save(tempImgStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            byte[] bytes = tempImgStream.GetBuffer();

                            return Convert.ToBase64String(bytes);
                        }
                    } else {
                        throw new ApplicationException("Page preview not found");
                    }
                } catch (Exception ex) {
                    _log.WriteErrorLog("Error connecting to the Imaging Server", ex);
                    throw;
                }
            }
        }
        #endregion
    }
}
