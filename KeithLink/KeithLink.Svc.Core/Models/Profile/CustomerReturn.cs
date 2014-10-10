using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="AccountReturn")]
    public class CustomerReturn
    {
        #region attributes
        private List<Customer> _customerList;
        #endregion

        #region ctor
        public CustomerReturn()
        {
            _customerList = new List<Customer>();
        }
        #endregion

        #region properties
        [DataMember(Name= "customers")]
        public List<Customer> Customers {
            get { return _customerList; }
            set { _customerList = value; }
        }
        #endregion
    }
}
