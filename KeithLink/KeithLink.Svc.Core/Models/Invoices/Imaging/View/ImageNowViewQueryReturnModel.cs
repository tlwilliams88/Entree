using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices.Imaging.View {
    public class ImageNowViewQueryReturnModel {
        public List<ResultRow> resultRows { get; set; }
        public bool hasMore { get; set; }
    }
}
