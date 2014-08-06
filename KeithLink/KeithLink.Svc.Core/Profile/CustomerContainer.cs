using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    [DataContract(Name="CustomerContainer")]
    public class CustomerContainer
    {
        #region attributes
        private StringBuilder _name;
        #endregion

        #region ctor
        public CustomerContainer()
        {
            _name = new StringBuilder();
        }

        public CustomerContainer(string CustomerName)
        {
            Name = CustomerName;
        }
        #endregion

        #region properties
        [DataMember(Name="Name")]
        public string Name
        {
            get { return _name.ToString(); }
            set { _name = new StringBuilder(value); }
        }
        #endregion
    }
}
