﻿using KeithLink.Svc.Core.Models.Profile;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface ICustomerDomainRepository {
        KeithLink.Svc.Core.Models.Authentication.AuthenticationModel AuthenticateUser(string userName, string password);
        //bool AuthenticateUser(string userName, string password, out string errorMessage);

        string CreateUser(string customerName, string emailAddress, string password, string firstName, string lastName, string roleName, List<string> permissions);

        void DeleteUser(string emailAddress);

        void ExpirePassword( string emailAddress );
        
        string GetNewUserName(string emailAddress);

        UserPrincipal GetUser(string userName);
        
        string GetUserGroup(string userName, List<string> groupNames);

        List<string> GetUserPermissions(string userName, List<string> groupNames);

        void SetUserPermissions(string userName, List<string> updatePermits, string updater);

        void GrantAccess(string grantedBy, string userName, string roleName);

        bool HasAccess(string userName, string roleName);

        bool IsPasswordExpired( string emailAddress );

        void JoinGroup(string customerName, string roleName, UserPrincipal user);

		void RevokeAccess(string revokedBy, string userName, string roleName);

        void UnlockAccount(string emailAddress);

        //void UpdateUserGroups(List<string> customerNames, string roleName, string userEmail);

		bool UpdatePassword(string updatedBy, string emailAddress, string oldPassword, string newPassword);

        void UpdatePassword( string emailAddress, string newPassword );

        void UpdateUserAttributes(string oldEmailAddress, string newEmailAdress, string firstName, string lastName);

        bool UsernameExists(string userName);
    }
}
