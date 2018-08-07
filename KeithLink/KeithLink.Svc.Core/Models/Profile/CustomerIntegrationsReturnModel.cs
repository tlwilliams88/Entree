using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name = "customer")]
	[Serializable]
    public class CustomerIntegrationsReturnModel
    {
        [DataMember(Name = "customernumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "customername")]
        public string CustomerName { get; set; }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }


    }

}
