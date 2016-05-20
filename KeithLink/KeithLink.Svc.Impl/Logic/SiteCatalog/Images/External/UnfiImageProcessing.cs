using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Models.SiteCatalog.Products.External;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace KeithLink.Svc.Impl.Logic.SiteCatalog.Images.External
{
    public class UnfiImageProcessing : IExternalImageProcessorUnfi
    {
        public int index;
        private readonly IEventLogRepository _log;
        private List<Task> imageScaleTasks;
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
        private readonly string IX_ONE_PRODUCTIMAGE_GET_URL =
            "https://exchange.ix-one.net/services/ImageHandler.aspx?FileName={0}&Type=JPG&Size=MEDIUM";

        public UnfiImageProcessing(IEventLogRepository log)
        {
            _log = log;
        }

        public void StartProcessAllImages()
        {
            imageScaleTasks = new List<Task>();

            // recursively create directories for saving images if they don't exist
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesRepo);

            List<DataRow> list = GetUnfiProductsInOurData();
            _log.WriteInformationLog(" Total we have in staging is " + list.Count);

            // given our list of items get the list of products they have on file
            IxOneReturn received = GetIXOneList(list);
            {
                Dictionary<string, IxOneProduct> dict = new Dictionary<string, IxOneProduct>();
                foreach (IxOneProduct prod in received.Products)
                {
                    if (dict.ContainsKey(prod.UPC) == false)
                    {
                        dict.Add(prod.UPC, prod);
                    }
                }
                received.Products.Clear();
                received.Products.AddRange(dict.Values);
            }
            _log.WriteInformationLog(" Total they have on file is " + received.Products.Count);

            index = 0;
            Parallel.ForEach(received.Products, new ParallelOptions { MaxDegreeOfParallelism = 8 }, product =>
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
                            thisName += "_AAAA.JPG";
                        }
                        else
                        {
                            thisName = filename.Replace(".TIF", ".JPG");
                        }
                        ProcessItem(thisName, filename);
                    }
                }
            });
            _log.WriteInformationLog(" Download Complete");
            imageScaleTasks.Clear();
        }

        private List<DataRow> GetUnfiProductsInOurData()
        {
            Dictionary<string, DataRow> dict = new Dictionary<string, DataRow>();
            // get our list of items from etl staging
            var repo = new KeithLink.Svc.Impl.ETL.StagingRepositoryImpl(_log);
            var itemTable = repo.ReadUNFIItems();
            foreach (DataRow row in itemTable.Rows)
            {
                if(dict.ContainsKey(row.GetString("RetailUPC")) == false)
                {
                    dict.Add(row.GetString("RetailUPC"), row);
                }
            }
            return dict.Values.ToList();
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

         private IxOneReturn GetIXOneList(List<DataRow> items)
        {            
            IxOneReturn received = null;
            if (items.Count > 1000)
            {
                // recursively nest calls in blocks of 1000
                received = GetIXOneList(items.Skip(1000).ToList());
                items = items.Take(1000).ToList();
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://exchange.ix-one.net/services/Products/filtered");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers[HttpRequestHeader.Authorization] = "Bearer " + 
                Configuration.CatalogServiceUnfiImagesIxOneAuthToken;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                StringBuilder sbUPC = new StringBuilder();
                foreach (DataRow dr in items.Take(items.Count - 1))
                {
                    sbUPC.Append("'" + dr.GetString("RetailUPC") + "',");
                }
                {
                    DataRow dr = items[items.Count - 1];
                    sbUPC.Append("'" + dr.GetString("RetailUPC") + "'");
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

            IxOneReturn get = new IxOneReturn(result);
            if(received != null)
            {
                // add in products received in nested calls
                get.Products.AddRange(received.Products);
            }
        
            return get;
        }

        private byte[] GetIXOneImage(string filename)
        {
            try
            {
                string url = "https://exchange.ix-one.net/services/ImageHandler.aspx?FileName=" + filename + "&Type=PNG&Size=MEDIUM";
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
                _log.WriteErrorLog(string.Format("Unfi Download Image({0})", filename), ex);

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
