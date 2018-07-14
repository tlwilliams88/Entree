using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models
{
    [DataContract(Name="AccountReturn")]
    public class AccountReturn
    {
        #region attributes
        private List<Account> _accountList;
        #endregion

        #region ctor
        public AccountReturn()
        {
            _accountList = new List<Account>();
        }
        #endregion

        #region properties
        [DataMember(Name= "Accounts")]
        public List<Account> Accounts {
            get { return _accountList; }
            set { _accountList = value; }
        }
        #endregion
    }
}
