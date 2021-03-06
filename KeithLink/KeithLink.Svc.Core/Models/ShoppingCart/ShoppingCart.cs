﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
    [DataContract(Name = "ShoppingCart")]
    public class ShoppingCart
    {
        [DataMember(Name = "id")]
        public Guid CartId { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        public string BranchId { get; set; }
        [DataMember(Name = "requestedshipdate")]
        public string RequestedShipDate { get; set; }
        [DataMember(Name = "ponumber")]
        public string PONumber { get; set; }
        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "itemcount")]
        public int ItemCount { get; set; }

        [DataMember(Name = "listid")]
        public long? ListId { get; set; }
        [DataMember(Name = "listtype")]
        public ListType ListType { get; set; }

        [DataMember(Name = "piececount")]
        public int PieceCount { get; set; }

        [DataMember(Name = "subtotal")]
        public decimal SubTotal { get; set; }

        [DataMember(Name = "createddate")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "containsspecialitems")]
        public bool ContainsSpecialItems { get; set; }

        [DataMember(Name = "items")]
        public List<ShoppingCartItem> Items { get; set; }

        [DataMember(Name = "approval")]
        public ApprovedCartModel Approval { get; set; }
	}
}
