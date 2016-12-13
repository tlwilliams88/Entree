using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Models.SiteCatalog.Products.External
{
    public class IxOneReturn
    {
        private int index;
        private IEventLogRepository _log;

        private IxOneReturn() { }

        /// <summary>
        /// Give a list of UPCs, get the products from Ix-One that they have on file.
        /// </summary>
        /// <param name="items">The list of UPC12s that we have in our data</param>
        /// <returns></returns>
        public static IxOneReturn GetIXOneList(List<string> items)
        {
            IxOneReturn ret = new IxOneReturn();
            ret.GetIXOneListRecurse(items);
            return ret;
        }

        /// <summary>
        /// A count of the products they have on file
        /// </summary>
        public int ProductCount => Products.Count;

        /// <summary>
        /// A count of the image filenames attached to the products they have on file
        /// </summary>
        public int ProductImageCount => CountTotalImages();

        /// <summary>
        /// Download the images for all the files they claim to have
        /// </summary>
        public void DownloadImagesForProducts(IEventLogRepository log)
        {
            index = 0;
            _log = log;
            Parallel.ForEach(Products, new ParallelOptions { MaxDegreeOfParallelism = 16 }, product =>
            {
                index++;
                if (product.Filenames.Count > 0)
                {
                    string best = ChooseBestFilename(product);
                    foreach (string filename in product.Filenames)
                    {

                        int trial = 0;
                        try
                        {
                            GetItem(best, filename, Constants.IXONE_PRODUCTIMAGE_PREFERREDTYPE);
                        }
                        catch// (Exception ex)
                        {
                            trial++;
                            if (trial == 1)
                            {
                                GetItem(best, filename, Constants.IXONE_PRODUCTIMAGE_BACKUPTYPE);
                            }
                        }
                    }
                }
                if (index % 100 == 0)
                {
                    _log.WriteInformationLog(string.Format(" Downloaded {0} products from Ix-One", index));
                }
            });
        }

        private void GetItem(string best, string filename, string imagetype)
        {
            string thisName;
            bool isdownloaded = false;
            if (filename.IndexOf('_') > -1 && filename.Length > filename.IndexOf('_') + 5)
            {
                if (Configuration.CatalogServiceUnfiImagesIxOneImagesWeTake.Any(s => s == filename.Substring(filename.IndexOf('_') + 1, 4)))
                {
                    isdownloaded = true;
                }
            }

            // Set the best image to be the first alphabetically
            if (filename.Equals(best, StringComparison.CurrentCultureIgnoreCase))
            {
                thisName = filename.Substring(0, filename.IndexOf("_"));
                thisName += "_APRI." + imagetype;
            }
            else
            {
                thisName = filename.Replace(".TIF", "." + imagetype);
            }
            if (isdownloaded)
            {
                ProcessItem(thisName, filename, imagetype);
            }
        }

        private void Define(string json)
        {
            JObject obj = JObject.Parse(json);
            JToken products = obj["Products"];
            Products = new List<IxOneProduct>();
            foreach (var prod in products.Children())
            {
                IxOneProduct product = new IxOneProduct();
                product.UPC = prod["UPC12"].ToString();
                JToken images = prod["StandardizedImage"];
                product.Filenames = new List<string>();
                foreach (var img in images.Children())
                {
                    product.Filenames.Add(img["OriginalFileName"].ToString());
                }
                Products.Add(product);
            }
        }

        private List<IxOneProduct> Products { get; set; }

        private int CountTotalImages()
        {
            int count = 0;
            foreach (var product in Products)
            {
                count += product.Filenames.Count;
            }
            return count;
        }

        private void ProcessItem(string savedas, string filename, string imagetype)
        {
            try
            {
                FileInfo fi = new FileInfo(Configuration.CatalogServiceUnfiImagesRepo + "\\" + savedas);
                if (!fi.Exists)
                {
                    byte[] buf = GetIXOneImage(filename, imagetype);
                    if (buf != null)
                    {
                        Image img = byteArrayToImage(buf);
                        if (img != null)
                        {
                            Image img2 = (Image)img.Clone();
                            System.Drawing.Imaging.ImageFormat imgformat = null;
                            if (imagetype.Equals("JPG"))
                            {
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                            }
                            else if(imagetype.Equals("PNG"))
                            {
                                imgformat = System.Drawing.Imaging.ImageFormat.Png;
                            }
                            //img2.Save(Configuration.CatalogServiceUnfiImagesRepo + "\\" + savedas,
                            //    imgformat);
                            using (MemoryStream memory = new MemoryStream())
                            {
                                using (FileStream fs = new FileStream(Configuration.CatalogServiceUnfiImagesRepo + "\\" + savedas, 
                                                                      FileMode.Create, 
                                                                      FileAccess.ReadWrite))
                                {
                                    img2.Save(memory, imgformat);
                                    byte[] bytes = memory.ToArray();
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog(string.Format("ProcessItem savedas={0} filename={1} imagetype={2}", savedas, filename, imagetype), ex);
                throw ex;
            }
        }

        private string ChooseBestFilename(IxOneProduct product)
        { // priority is from bottom up
            string filename = "";
            foreach (string ext in Configuration.CatalogServiceUnfiImagesIxOneImagesWeTake)
            {
                foreach (string filen in product.Filenames)
                {
                    if (filen.IndexOf(ext) > -1)
                    {
                        filename = filen;
                        break;
                    }
                }
                if (filename.Length > 0) { break; }
            }
            return filename;
        }

        private void GetIXOneListRecurse(List<string> items)
        {
            IxOneReturn received = null;
            if (items.Count > 1000)
            {
                // recursively nest calls in blocks of 1000
                received = GetIXOneList(items.Skip(1000).ToList());
                items = items.Take(1000).ToList();
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.IXONE_PRODUCTINFO_GET_URL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers[HttpRequestHeader.Authorization] = "Bearer " +
                Configuration.CatalogServiceUnfiImagesIxOneAuthToken;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                StringBuilder sbUPC = new StringBuilder();
                foreach (string upc in items.Take(items.Count - 1))
                {
                    sbUPC.Append("'" + upc + "',");
                }
                {
                    string upc = items[items.Count - 1];
                    sbUPC.Append("'" + upc + "'");
                }

                // build dynamic request for images and then serialize as json
                dynamic request = new ExpandoObject();
                request.PageNumber = 1;
                request.PageSize = 1000;
                request.DataFilters = new List<ExpandoObject>();
                request.DataFilters.Add(new ExpandoObject());
                request.DataFilters[0].EntityName = "Product";
                request.DataFilters[0].PropertyName = "UPC12";
                request.DataFilters[0].Operator = "IN";
                request.DataFilters[0].Comparator = sbUPC.ToString();
                request.PropertyListing = new List<ExpandoObject>();
                request.PropertyListing.Add(new ExpandoObject());
                request.PropertyListing[0].EntityName = "Product";
                request.PropertyListing[0].PropertyName = "UPC12";
                request.PropertyListing.Add(new ExpandoObject());
                request.PropertyListing[1].EntityName = "Product";
                request.PropertyListing[1].PropertyName = "StandardizedImage";
                request.PropertyListing.Add(new ExpandoObject());
                request.PropertyListing[2].EntityName = "StandardizedImage";
                request.PropertyListing[2].PropertyName = "OriginalFileName";

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            string result;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Define(result);
            if (received != null)
            {
                // add in products received in nested calls
                Products.AddRange(received.Products);
            }
        }

        private byte[] GetIXOneImage(string filename, string imagetype)
        {
            string url = null;
            try
            {
                url = string.Format(Constants.IXONE_PRODUCTIMAGE_GET_URL, filename, imagetype);
                //_log.WriteInformationLog(" Url is " + url);
                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " +
                    Configuration.CatalogServiceUnfiImagesIxOneAuthToken;
                byte[] bytes = client.DownloadData(url);

                return bytes;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception caught in download image: {0}",
                //                  ex.ToString());
                _log.WriteErrorLog(string.Format("Unfi Download Image({0})", url), ex);

                return null;
            }
        }
        private Image byteArrayToImage(byte[] bytesArr)
        {
            try
            {
                MemoryStream memstr = new MemoryStream(bytesArr);
                Image img = Image.FromStream(memstr);
                return img;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception caught in byteArrayToImage: {0}",
                //                  ex.ToString());
                _log.WriteErrorLog("Unfi byteArrayToImage", ex);

                return null;
            }
        }
    }
}
