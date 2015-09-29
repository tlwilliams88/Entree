// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart {
    [DataContract]
    public class ShoppingCartItemReportModel {

        #region attributes 

        private string _notes;

        #endregion

        #region constructor

        public ShoppingCartItemReportModel() {
            _notes = String.Empty;
        }

        #endregion

        #region properties

        [DataMember]
        public string ItemNumber { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string PackSize { get; set; }

        [DataMember]
        public string Notes {
            get { return this._notes; }
            set {
                if (value != null) {
                    this._notes = value;
                } else {
                    this._notes = String.Format( "{0}...", value.Substring( 0, 12 ) );
                }
            }
        }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public decimal ParLevel { get; set; }

        [DataMember]
        public string Quantity { get; set; }

        [DataMember]
        public bool? Each { get; set; }

        [DataMember]
        public string CasePrice { get; set; }

        [DataMember]
        public string PackagePrice { get; set; }

        [DataMember]
        public string ExtPrice { get; set; }

        #endregion
    }
}
