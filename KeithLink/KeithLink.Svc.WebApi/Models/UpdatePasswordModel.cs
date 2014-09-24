using System;
using System.Text;

namespace KeithLink.Svc.WebApi.Models {
    public class UpdatePasswordModel {
        public string Email { get; set; }
        public string OriginalPassword { get; set; }
        public string NewPassword { get; set; }
    }
}