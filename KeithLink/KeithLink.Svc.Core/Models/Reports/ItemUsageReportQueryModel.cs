using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeithLink.Svc.Core.Models.Reports
{
    public class ItemUsageReportQueryModel
    {
        public UserSelectedContext UserSelectedContext { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public string sortDir { get; set; }
        public string sortField { get; set; }
    }
}
