using System;

namespace Entree.Core.Lists.Models.CustomList {
    public class CustomListHeader : BaseListHeader {
        public bool Active { get; set; }
        public string Name { get; set; }
        public Guid? UserId { get; set; }
    }
}
