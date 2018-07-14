using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Profile.Models
{
    [DataContract(Name="AccountUsersReturn")]
    public class AccountUsersReturn
    {
        #region ctor
        public AccountUsersReturn()
        {
        }
        #endregion

        #region properties
        [DataMember(Name = "AccountUsers")]
        public List<UserProfile> AccountUserProfiles { get; set; }

        [DataMember(Name = "CustomerUsers")]
        public List<UserProfile> CustomerUserProfiles { get; set; }
        #endregion
    }
}
