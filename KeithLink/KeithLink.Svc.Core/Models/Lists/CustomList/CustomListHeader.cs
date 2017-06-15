using System;

namespace KeithLink.Svc.Core.Models.Lists.CustomList {
    public class CustomListHeader : BaseListHeader {
        public bool Active { get; set; }
        public string Name { get; set; }
        public Guid? UserId { get; set; }
    }
}
