using System;

namespace KeithLink.Svc.WebApi.Models
{
    public class AccountModel
    {
        public string NationalOrRegionalAccountNumber { get; set; }
        public string Name { get; set; }
        public string AccountAdminEmail { get; set; }
    }
}