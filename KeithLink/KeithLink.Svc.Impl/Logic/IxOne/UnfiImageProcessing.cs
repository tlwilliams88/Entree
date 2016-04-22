using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Models.IxOne;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.IxOne
{
    public class UnfiImageProcessing
    {
        public static int index;
        private static IEventLogRepository _log;
        private static List<Task> imageScaleTasks;
        private static readonly string FILETYPE_STANDARDRES = "A";
        private static readonly string FILETYPE_HIGHRES = "C";
        private static readonly string FACING_FRONT = "1";
        private static readonly string FACING_LEFT = "2";
        private static readonly string FACING_TOP = "3";
        private static readonly string FACING_BACK = "7";
        private static readonly string FACING_RIGHT = "8";
        private static readonly string FACING_DISPLAY = "D";
        private static readonly string FACING_NUTRITION = "N";
        private static readonly string FACING_INGREDIENTS = "I";
        private static readonly string ANGLE_CENTER = "C";
        private static readonly string ANGLE_LEFT = "L";
        private static readonly string ANGLE_RIGHT = "R";
        private static readonly string ANGLE_NOPLUNGE = "N";
        private static readonly string PACK_IN = "1";
        private static readonly string PACK_OUT = "0";
        private static readonly string PACK_CASE = "A";
        private static readonly string PACK_INNER = "B";
        private static readonly string PACK_PREPARED = "D";
        public static void StartProcessAllImages(IEventLogRepository log)
        {
            _log = log;
            imageScaleTasks = new List<Task>();
            List<DataRow> list = new List<DataRow>();
            var repo = new KeithLink.Svc.Impl.ETL.StagingRepositoryImpl(_log);

            // recursively create directories for saving images if they don't exist
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesRepo);
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesNewOnlyDir);
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesNewOnlyDirThumbs);

            // get our list of items from etl staging
            var itemTable = repo.ReadUNFIItems();
            foreach (DataRow row in itemTable.Rows)
            {
                //list.Add(row.GetString("RetailUPC"), row.GetString("ItemNumber"));
                list.Add(row);
            }
            _log.WriteInformationLog(" Total we have in staging is " + list.Count);

            // given our list of items get the list of products they have on file
            IxOneReturn received = GetIXOneList(list);
            _log.WriteInformationLog(" Total they have on file is " + received.Products.Count);

            index = 0;
            Parallel.ForEach(received.Products, new ParallelOptions { MaxDegreeOfParallelism = 8 }, product =>
            {
                index++;
                if (product.Filenames.Count > 0)
                {
                    ProcessItem(product.UPC, ChooseBestFilename(product));
                }
                if (index * 10 == 0) { _log.WriteInformationLog(" Processed " + index); }
            });
            _log.WriteInformationLog(" Download Complete");
            imageScaleTasks.Clear();
        }

        private static void ProcessItem(string gtin12, string filename)
        {
            try
            {
                FileInfo fi = new FileInfo(Configuration.CatalogServiceUnfiImagesRepo + "\\" + gtin12 + ".jpg");
                if (!fi.Exists)
                {
                    byte[] buf = GetIXOneImage(filename);
                    if (buf != null)
                    {
                        Image img = byteArrayToImage(buf);
                        if (img != null)
                        {
                            Image img2 = (Image)img.Clone();
                            img.Save(Configuration.CatalogServiceUnfiImagesRepo + "\\" + gtin12 + ".jpg",
                                System.Drawing.Imaging.ImageFormat.Jpeg);
                            img.Save(Configuration.CatalogServiceUnfiImagesNewOnlyDir + "\\" + gtin12 + ".jpg",
                                System.Drawing.Imaging.ImageFormat.Jpeg);
                            if (Configuration.CatalogServiceUnfiImagesMakeThumbnails.Equals
                                ("true", StringComparison.CurrentCultureIgnoreCase))
                            {
                                ScaleImage(Configuration.CatalogServiceUnfiImagesNewOnlyDir + "\\" + gtin12 + ".jpg");
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                _log.WriteErrorLog("ProcessItem", ex);
            }
        }

        private static string ChooseBestFilename(IxOneProduct product)
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
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_IN + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_IN + ".TIF")).First();
            if (product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_PREPARED + ".TIF")).Count() > 0)
                filename = product.Filenames.Where(f => f.EndsWith(FACING_FRONT + ANGLE_CENTER + PACK_PREPARED + ".TIF")).First();
            return filename;
        }

        private static IxOneReturn GetIXOneList(List<DataRow> items)
        {            
            IxOneReturn received = null;
            if (items.Count > 1000)
            {
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
                string json =
"{\"PageNumber\": 1," +
  "\"PageSize\": 1000," +
  "\"DataFilters\": [" +
    "{\"EntityName\": \"Product\"," +
     "\"PropertyName\": \"UPC12\"," +
     "\"Operator\": \"IN\"," +
     "\"Comparator\": \"" + sbUPC.ToString() + "\"" +
    "}" +
    "]," +
   "\"PropertyListing\": [" +
    "{\"EntityName\": \"Product\"," +
     "\"PropertyName\": \"UPC12\"" +
    "}," +
    "{\"EntityName\": \"Product\"," +
     "\"PropertyName\": \"StandardizedImage\"" +
    "}," +
    "{\"EntityName\": \"StandardizedImage\"," +
     "\"PropertyName\": \"OriginalFileName\"" +
    "}" +
   "]" +
 "}";

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
                //_log.WriteInformationLog(" Received " + get.Products.Count + " add to " + received.Products.Count);
                //foreach(var product in received.Products)
                //{
                //    _log.WriteInformationLog(" Received - " + product.ToString());
                //}
                get.Products.AddRange(received.Products);
            }
        
            return get;
        }

        private static byte[] GetIXOneImage(string filename)
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
                //_log.WriteErrorLog("Download Image", ex);

                return null;
            }
        }
        private static Image byteArrayToImage(byte[] bytesArr)
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
                //_log.WriteErrorLog("byteArrayToImage", ex);

                return null;
            }
        }

        private static void ScaleImage(string filename)
        {
            // Start the HandleFile method.
            string dir = filename.Substring(0, filename.LastIndexOf("\\"));
            string fname = filename.Substring(filename.LastIndexOf("\\") + 1);
            Image img = Image.FromFile(filename);
            img = ScaleImage(img, 150, 150);
            img.Save(Configuration.CatalogServiceUnfiImagesNewOnlyDirThumbs + "\\" + fname, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        /// <summary>
        /// Scales an image proportionally.  Returns a bitmap.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        private static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            Bitmap bmp = new Bitmap(newImage);

            return bmp;
        }
    }
}
