using System;
using System.Text;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.WebApi.Models
{
    public class UserProfileModel
    {
        public string UserId { get; set; }
        public string Email { get; set;}
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CustomerName { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
        public string Address { get; set; }
		public string Role { get; set; }
        public string BranchId { get; set; }
        public List<Customer> Customers { get; set; }
    }
}