using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using KeithLink.Svc.Core.Interface.DataConnection;

using System.Collections.Generic;
using System.Configuration;
using KeithLink.Common.Core.Interfaces;

using System;
using System.Data;
using System.Data.SqlClient;
using KeithLink.Svc.Core.Models.Documents;
using System.Net.Http;
using System.Net;

namespace KeithLink.Svc.Impl.Repository.DataConnection
{
    public class AzureContainerConnection : IAzureContainerConnection
    {
        private const string CONTAINER = "hns";

        private const string CONNECTION_STRING = "AzureConnection";
        private readonly string _connectionString;
        public AzureContainerConnection(string connection)
        {
            _connectionString = connection;
        }

        public List<DocumentReturnModel> GetDocuments(string identifier = "")
        {
            List<DocumentReturnModel> returnValue = new List<DocumentReturnModel>();
            CloudStorageAccount storageAccount;

            try
            {
                if (CloudStorageAccount.TryParse(_connectionString, out storageAccount))
                {
                    CloudBlobClient client = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = client.GetContainerReference("hns"); 

                    var directory = container.GetDirectoryReference(identifier);
                    IEnumerable<IListBlobItem> documents = directory.ListBlobs();

                    foreach (var document in documents)
                    {
                        DocumentReturnModel doc = new DocumentReturnModel();

                        var i = 0;
                        foreach (var segment in document.Uri.Segments)
                        {
                           doc.Name = System.Net.WebUtility.UrlDecode(document.Uri.Segments[i]);
                            i++;
                        }

                        if (doc.Name[doc.Name.Length -1].ToString() == "/")
                        {
                            var decodedUrl = System.Net.WebUtility.UrlDecode(document.Uri.AbsolutePath);
                            doc.Type = "folder";
                            doc.Name = doc.Name.Remove(doc.Name.Length - 1);
                            doc.Url = decodedUrl.Substring(decodedUrl.IndexOf(identifier), decodedUrl.Length - decodedUrl.IndexOf(identifier));
                        }
                        else
                        {
                            if (doc.Name.IndexOf(".xls") > 0) { doc.Type = "excel"; }
                            else if (doc.Name.IndexOf(".pdf") > 0) { doc.Type = "pdf"; }
                            else if (doc.Name.IndexOf(".docx") > 0) { doc.Type = "word"; }
                            else if (doc.Name.IndexOf(".txt") > 0) { doc.Type = "text"; }
                            else if (doc.Name.IndexOf(".jpg") > 0 || doc.Name.IndexOf(".png") > 0 || doc.Name.IndexOf(".bmp") > 0) { doc.Type = "image"; }
                            else if (doc.Name.IndexOf(".zip") > 0) { doc.Type = "zip"; }
                            else { doc.Type = "other"; }

                            doc.Url = "https://bekcdn.azureedge.net/bekblob" + System.Net.WebUtility.UrlDecode(document.Uri.AbsolutePath);
                        }

                        if (doc.Type != "folder")
                        {
                            using (HttpClient http = new HttpClient())
                            {
                                using (HttpResponseMessage response = http.GetAsync("https://bekcdn.azureedge.net/bekblob" + System.Net.WebUtility.UrlDecode(document.Uri.AbsolutePath) + "?comp=metadata").Result)
                                {
                                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                                    {
                                        doc.Modified = response.Content.Headers.LastModified;
                                    }
                                }
                            }
                        }
                        returnValue.Add(doc);
                    }                    
                }
                else
                {
                    throw new System.Exception("Failed to parse Azure connection string");
                }

                return returnValue;
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException e)
            {
                return new List<DocumentReturnModel>();
            }
        }
    }
}