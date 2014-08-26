using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Profile
{
    [DataContract(Name="CustomerContainer")]
    public class CustomerContainer
    {
        #region attributes
         #endregion

        #region ctor
        public CustomerContainer(string CustomerName)
        {
            Name = CustomerName;
        }
        #endregion

        #region properties
        [DataMember(Name="Name")]
		public string Name { get; set; }
        #endregion
    }
}
