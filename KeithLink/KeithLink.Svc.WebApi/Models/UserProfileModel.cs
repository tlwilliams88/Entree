using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models
{
    public class UserProfileModel
    {
        public string UserId { get; set; }
        public string Email { get; set;}
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CustomerName { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
		public string RoleName { get; set; }
        public string BranchId { get; set; }
    }
}