using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart {
    [DataContract]
    public class ShoppingCartReportModel {

        [DataMember]
        public string CartName { get; set; }

        [DataMember]
        public string ListName { get; set; }

        [DataMember]
        public List<ShoppingCartItemReportModel> CartItems { get; set; }

        [DataMember]
        public List<ShoppingCartItemReportModel> ListItems { get; set; }

    }
}
