using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    [DataContract(Name="UserProfileReturn")]
    public class UserProfileReturn
    {
        #region attributes
        private List<UserProfile> _profileList;
        #endregion

        #region ctor
        public UserProfileReturn()
        {
            _profileList = new List<UserProfile>();
        }
        #endregion

        #region properties
        [DataMember(Name= "UserProfiles")]
        public List<UserProfile> UserProfiles {
            get { return _profileList; }
            set { _profileList = value; }
        }
        #endregion
    }
}
