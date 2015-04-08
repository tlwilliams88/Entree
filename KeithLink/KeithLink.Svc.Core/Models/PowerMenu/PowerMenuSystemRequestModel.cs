using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("PowerNetUserRequest")]
    public class PowerMenuSystemRequestModel {
        #region attributes

        public enum Operations {
            Add,
            Delete,
            Edit
        }

        #endregion

        #region constructor

        public PowerMenuSystemRequestModel() {
            Login = new PowerMenuSystemRequestAdminModel();
            User = new PowerMenuSystemRequestUserModel();
            Operation = Operations.Edit;
        }

        #endregion

        #region properties

        public PowerMenuSystemRequestAdminModel Login { get; set; }
        public PowerMenuSystemRequestUserModel User { get; set; }
        public Operations Operation { get; set; }

        #endregion
    }
}
