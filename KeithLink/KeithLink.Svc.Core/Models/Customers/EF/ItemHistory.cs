using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeithLink.Svc.Core.Models.EF;

namespace KeithLink.Svc.Core.Models.Customers.EF {
    public class ItemHistory : BaseEFModel {

        [MaxLength( 3 )]
        [Column( TypeName = "char" )]
        [Index( "IdxItemHistory" , 1)]
        public string BranchId { get; set; }

        [MaxLength( 6 )]
        [Column( TypeName = "char" )]
        [Index( "IdxItemHistory" , 2)]
        public string CustomerNumber { get; set; }

        [MaxLength( 6 )]
        [Column( TypeName = "char" )]
        [Index( "IdxItemHistory" , 3)]
        public string ItemNumber { get; set; }

        public int EightWeekAverage { get; set; }

    }
}
