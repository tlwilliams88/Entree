﻿using KeithLink.Svc.Core.Models.Profile;
using System.DirectoryServices.AccountManagement;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ICustomerDomainRepository {
        KeithLink.Svc.Core.Models.Authentication.AuthenticationModel AuthenticateUser(string userName, string password);
        //bool AuthenticateUser(string userName, string password, out string errorMessage);

        string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName);

        string GetNewUserName(string emailAddress);

        UserPrincipal GetUser(string userName);

        void ExpirePassword( string emailAddress );

        bool IsPasswordExpired( string emailAddress );

        bool IsInGroup(string userName, string groupName);

        void JoinGroup(string customerName, string roleName, UserPrincipal user);

        bool UpdatePassword(string emailAddress, string oldPassword, string newPassword);

        void UpdateUserAttributes(string oldEmailAddress, string newEmailAdress, string firstName, string lastName);

        bool UsernameExists(string userName);
    }
}
