using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="CustomerContainerReturn")]
    public class CustomerContainerReturn
    {
        #region attributes
        private List<CustomerContainer> _customers;
        #endregion

        #region ctor
        public CustomerContainerReturn()
        {
            _customers = new List<CustomerContainer>();
        }
        #endregion

        #region properties
        [DataMember(Name="CustomerContainers")]
        public List<CustomerContainer> CustomerContainers
        {
            get { return _customers; }
            set { _customers = value; }
        }
        #endregion
    }
}
