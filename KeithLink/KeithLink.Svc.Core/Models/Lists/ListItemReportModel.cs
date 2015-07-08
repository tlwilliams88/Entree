using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
	[DataContract]
	public class ListItemReportModel {
        #region attributes

        private string _notes;

        #endregion

        public ListItemReportModel() {
            _notes = String.Empty;
        }

        #region properties

        [DataMember]
		public string ItemNumber { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string PackSize { get; set; }
		[DataMember]
		public int InCart { get; set; }
		[DataMember]
		public string Brand { get; set; }
        [DataMember]
        public decimal ParLevel { get; set; }

        /// <summary>
        /// Report should only show 15 characters and if longer needs to mark it as a continuation.
        /// </summary>
        [DataMember]
        public string Notes {
            get { return this._notes; }
            set {
                if (value != null) {
                    if (value.Length < 15) {
                        this._notes = value;
                    } else {
                        this._notes = String.Format( "{0}...", value.Substring( 0, 12 ) );
                    }
                }
            }
        }

        #endregion
    }
}
