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
        private readonly IEventLogRepository _log;

        public UnfiImageProcessing(IEventLogRepository log)
        {
            _log = log;
        }

        public void StartProcessAllImages()
        {
            // recursively create directories for saving images if they don't exist
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesRepo);

            List<string> list = GetUnfiProductsInOurData().DistinctByUPC();
            _log.WriteInformationLog(" Total Unique UNFI products we have in staging is " + list.Count);

            // given our list of items get the list of products they have on file
            IxOneReturn received = IxOneReturn.GetIXOneList(list);
            _log.WriteInformationLog(string.Format(" Of our UNFI products they have {0} on file", received.ProductCount));
            _log.WriteInformationLog(string.Format(" For their UNFI products they have {0} images on file", received.ProductImageCount));

            received.DownloadImagesForProducts(_log);
            _log.WriteInformationLog(" Download Complete");
        }

        private List<string> GetUnfiProductsInOurData()
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
            return dict.Keys.ToList();
        }
    }

    public static class SpecificListExtensions
    {
        public static List<string> DistinctByUPC(this List<string> list)
        {
            List<string> newList = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string upc in list)
            {
                if (dict.ContainsKey(upc) == false)
                {
                    dict.Add(upc, upc);
                }
            }
            newList.AddRange(dict.OrderBy(d => d.Key).Select(d => d.Key).ToList());
            return newList;
        }
    }
}
