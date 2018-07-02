using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.UserFeedback
{
    [DataContract(Name = "userFeedbackReturn")]
    public class UserFeedbackReturn
    {
        #region attributes
        private List<UserFeedback> _userFeedbackList;
        #endregion

        #region ctor
        public UserFeedbackReturn()
        {
            _userFeedbackList = new List<UserFeedback>();
        }
        #endregion

        #region properties
        [DataMember(Name = "UserFeedbacks")]
        public List<UserFeedback> UserFeedbacks
        {
            get { return _userFeedbackList; }
            set { _userFeedbackList = value; }
        }
        #endregion
    }
}
