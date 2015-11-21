using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Configuration.EF
{
    public enum ExternalCatalogType
    {
        BEK = 0,
        UNFI = 1
    }

    public class ExternalCatalog : BaseEFModel
    {
        [MaxLength(24)]
        public string BekBranchId { get; set; }
        [MaxLength(24)]
        public string ExternalBranchId { get; set; }
        public ExternalCatalogType Type { get; set; }
    }
}

