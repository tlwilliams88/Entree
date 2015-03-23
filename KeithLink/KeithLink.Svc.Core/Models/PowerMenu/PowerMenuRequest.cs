using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace KeithLink.Svc.Core.Models.PowerMenu {
    [XmlRoot("PowerNetUserRequest")]
    public class PowerMenuRequest {
        #region attributes

        public enum Operations {
            Create,
            Delete,
            Edit
        }

        #endregion

        #region constructor

        public PowerMenuRequest() {
            Login = new PowerMenuRequestLogin();
            User = new PowerMenuRequestUser();
            Operation = Operations.Edit;
        }

        #endregion

        #region properties

        public PowerMenuRequestLogin Login { get; set; }
        public PowerMenuRequestUser User { get; set; }
        public Operations Operation { get; set; }

        #endregion
    }
}
