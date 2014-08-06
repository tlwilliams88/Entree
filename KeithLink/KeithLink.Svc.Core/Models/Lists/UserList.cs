using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Lists
{
    [DataContract(Name = "UserList")]
    public class UserList
    {
        [DataMember(Name = "listid")]
        public Guid ListId { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "items")]
        public List<ListItem> Items { get; set; }
    }
}
