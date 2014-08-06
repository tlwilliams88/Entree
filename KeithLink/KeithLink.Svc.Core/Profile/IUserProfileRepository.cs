﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Profile
{
    public interface IUserProfileRepository
    {
        public void CreateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber);
        public void DeleteUserProfile(string userName);
        public UserProfile GetUserProfile(string userName);
        public void UpdateUserProfile(string userName, string customerName, string emailAddres, string firstName, string lastName, string phoneNumber);
    }
}
