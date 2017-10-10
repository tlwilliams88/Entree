using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Models.SiteCatalog.Products.External;

namespace KeithLink.Svc.Impl.Logic.SiteCatalog.Images.External {
    public class UnfiImageProcessing : IExternalImageProcessorUnfi {
        private readonly IEventLogRepository _log;

        public UnfiImageProcessing(IEventLogRepository log) {
            _log = log;
        }

        public void StartProcessAllImages() {
            // recursively create directories for saving images if they don't exist
            Directory.CreateDirectory(Configuration.CatalogServiceUnfiImagesRepo);

            List<string> list = GetUnfiProductUpcsInOurData();
            _log.WriteInformationLog(" Total Unique UNFI products we have in staging is " + list.Count);

            try {
                IxOneReturn received = IxOneReturn.GetIXOneList(list);
                _log.WriteInformationLog(string.Format(" Of our UNFI products they have {0} on file", received.ProductCount));
                _log.WriteInformationLog(string.Format(" For their UNFI products they have {0} images on file", received.ProductImageCount));

                // given our list of items get the list of products they have on file
                received.DownloadImagesForProducts(_log);
                _log.WriteInformationLog(" Download Complete");
            } catch (Exception ex) {
                _log.WriteErrorLog(string.Format("Download from IX-One failed: {0}", ex.Message), ex);
            }
        }

        private List<string> GetUnfiProductUpcsInOurData() {
            Dictionary<string, DataRow> dict = new Dictionary<string, DataRow>();
            // get our list of items from etl staging
            StagingRepositoryImpl repo = new StagingRepositoryImpl(_log);
            DataTable itemTable = repo.ReadUNFIItems();
            foreach (DataRow row in itemTable.Rows) {
                if (dict.ContainsKey(row.GetString("RetailUPC")) == false) {
                    dict.Add(row.GetString("RetailUPC"), row);
                }
            }
            // reorders the list of upcs in alphabetical list
            return dict.OrderBy(d => d.Key)
                       .Select(d => d.Key)
                       .ToList();
        }
    }
}