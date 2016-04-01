using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Profile;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Profile
{
    public interface IUserProfileRepository
    {
		void CreateUserProfile(string createdBy, string emailAddress, string firstName, string lastName, string phoneNumber, string branchId);

        void DeleteUserProfile(string userName);        
        
        CS.UserProfile GetCSProfile(string emailAddress);  
              
        CS.UserProfile GetCSProfile(Guid userId);

		void UpdateUserProfile(string updatedBy, Guid id, string emailAddres, string firstName, string lastName, string phoneNumber, string branchId);

        List<UserProfile> GetUsersForCustomerOrAccount(Guid orgId);

		List<CS.UserProfile> GetCSProfileForInternalUsers();
    }
}
