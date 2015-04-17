using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("PowerNetUserRequest")]
    [DataContract(Name = "PowerNetUserRequest", Namespace = "")]
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

        [DataMember(Name = "Login")]
        public PowerMenuSystemRequestAdminModel Login { get; set; }

        [DataMember(Name = "User")]
        public PowerMenuSystemRequestUserModel User { get; set; }

        [DataMember(Name = "Operation")]
        public Operations Operation { get; set; }

        #endregion
    }
}
