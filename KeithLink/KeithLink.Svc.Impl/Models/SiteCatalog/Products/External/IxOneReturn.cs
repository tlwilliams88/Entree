using KeithLink.Common.Core.Interfaces.Logging;
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
                        string thisName = null;
                        // Set the best image to be the first alphabetically
                        if (filename.Equals(best, StringComparison.CurrentCultureIgnoreCase))
                        {
                            thisName = filename.Substring(0, filename.IndexOf("_"));
                            thisName += "_APRI.JPG";
                        }
                        else
                        {
                            thisName = filename.Replace(".TIF", ".JPG");
                        }
                        ProcessItem(thisName, filename);
                    }
                }
                if (index % 100 == 0)
                {
                    _log.WriteInformationLog(string.Format(" Downloaded {0} products from Ix-One", index));
                }
            });
        }

        private IxOneReturn() { }
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

        private int index;
        private IEventLogRepository _log;
        private readonly string FILETYPE_STANDARDRES = "A";
        private readonly string FILETYPE_HIGHRES = "C";
        private readonly string FACING_FRONT = "1";
        private readonly string FACING_LEFT = "2";
        private readonly string FACING_TOP = "3";
        private readonly string FACING_BACK = "7";
        private readonly string FACING_RIGHT = "8";
        private readonly string FACING_DISPLAY = "D";
        private readonly string FACING_NUTRITION = "N";
        private readonly string FACING_INGREDIENTS = "I";
        private readonly string ANGLE_CENTER = "C";
        private readonly string ANGLE_LEFT = "L";
        private readonly string ANGLE_RIGHT = "R";
        private readonly string ANGLE_NOPLUNGE = "N";
        private readonly string PACK_IN = "1";
        private readonly string PACK_OUT = "0";
        private readonly string PACK_CASE = "A";
        private readonly string PACK_INNER = "B";
        private readonly string PACK_PREPARED = "D";
        private readonly string IX_ONE_PRODUCTINFO_GET_URL =
            "https://exchange.ix-one.net/services/Products/filtered";
        private readonly string IX_ONE_PRODUCTIMAGE_GET_URL =
            "https://exchange.ix-one.net/services/ImageHandler.aspx?FileName={0}&Type=JPG&Size=MEDIUM";
        private List<IxOneProduct> Products { get; set; }

        private int CountTotalImages()
        {
            int count = 0;
            foreach(var product in Products)
            {
                count += product.Filenames.Count;
            }
            return count;
        }

        private void ProcessItem(string savedas, string filename)
        {
            try
            {
                FileInfo fi = new FileInfo(Configuration.CatalogServiceUnfiImagesRepo + "\\" + savedas);
                if (!fi.Exists)
                {
                    byte[] buf = GetIXOneImage(filename);
                    if (buf != null)
                    {
                        Image img = byteArrayToImage(buf);
                        if (img != null)
                        {
                            Image img2 = (Image)img.Clone();
                            img.Save(Configuration.CatalogServiceUnfiImagesRepo + "\\" + savedas,
                                System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ProcessItem", ex);
            }
        }

        private string ChooseBestFilename(IxOneProduct product)
        { // priority is from bottom up
            string filename = product.Filenames[0];
            if (product.Filenames.Where(f => f.EndsWith(ANGLE_NOPLUNGE + PACK_IN + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(ANGLE_NOPLUNGE + PACK_IN + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(ANGLE_NOPLUNGE + PACK_PREPARED + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(ANGLE_NOPLUNGE + PACK_PREPARED + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(ANGLE_CENTER + PACK_IN + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(ANGLE_CENTER + PACK_IN + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(ANGLE_CENTER + PACK_PREPARED + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(ANGLE_CENTER + PACK_PREPARED + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_OUT + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_OUT + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_IN + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_IN + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_INNER + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_INNER + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_PREPARED + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_PREPARED + ".TIF")).First();
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

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(IX_ONE_PRODUCTINFO_GET_URL);
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

        private byte[] GetIXOneImage(string filename)
        {
            string url = null;
            try
            {
                url = string.Format(IX_ONE_PRODUCTIMAGE_GET_URL, filename);
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
