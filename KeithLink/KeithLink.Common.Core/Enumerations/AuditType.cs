using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Enumerations
{
	public enum AuditType
	{
		[Description("User Created")]
		UserCreated,
		[Description("User Update")]
		UserUpdate,
		[Description("Customer Group Created")]
		CustomerGroupCreated,
		[Description("Customer Group Deleted")]
		CustomerGroupDeleted,//System doesn't have this ability yet
		[Description("User Added To Customer Group")]
		UserAddedToCustomerGroup,
		[Description("User Removed From Customer Group")]
		UserRemovedFromCustomerGroup,
		[Description("User Assigned To Customer")]
		UserAssignedToCustomer,
		[Description("User Removed From Customer")]
		UserRemovedFromCustomer,
		[Description("Grant User Access")]
		GrantUserAccess,
		[Description("Revoke User Access")]
		RevokeUserAccess,
		[Description("Change Password")]
		ChagnePassword,
		[Description("Customer Added To Customer Group")]
		CustomerAddedToCustomerGroup,
		[Description("Customer Removed From Customer Group")]
		CustomerRemovedFromCustomerGroup,
	}
}
